const CardAbility = require('./cardAbilities/CardAbility');

class CardSlot {

    constructor(color) {
        this.color = color;
        this.attacks = 1;
        this.cards = [];
    }

    add(card) {
        this.cards.push(card);
    }

    getTotalPower() {
        let sum = 0;
        for (let i = 0; i < this.cards.length; i++) {
            const item = this.cards[i];
            if (item.canBattle()) {
                sum += item.getPower();
            }
        }
        return sum;
    }

    restoreSlotAttackCharges() {
        this.attacks = 1;
    }

    consumeCharge() {
        this.attacks -= 1;
    }

    getAttackCharges() {
        return this.attacks;
    }

    getLowestPower() {
        if (this.cards.length > 0) {
            let ret = null;
            let min = 9999;
            for(let i = 0; i < this.cards.length; i ++) {
                const current = this.cards[i];
                if (current.canBattle()) {
                    let power = current.getPower();
                    if (power < min) {
                        min = power;
                        ret = current;
                    }
                }
            }
            return ret;
        } else {
            return null;
        }
    }

    takeDamage(num) {
        const killed = [];
        const touched = [];
        if (this.cards.length > 0) {
            let exhausted = false;
            while (num > 0 && !exhausted) {
                const current = this.getLowestPower();
                if (current == null) {
                    exhausted = true;
                } else {
                    if (current.getPower() > num) {
                        // aborbed the damage
                        current.takeDamage(num);
                        touched.push(current);
                        num = 0;
                    } else {
                        // killed by this damage
                        num -= current.getPower();
                        current.takeDamage(num);
                        this.cards.splice(this.cards.indexOf(current), 1);
                        killed.push(current);
                    }
                }
            }
        }
        return {
            touched: touched,
            killed: killed
        };
    }

    checkBuffsEndTurn(caster) {
        for (let i = 0; i < this.cards.length; i++) {
            this.cards[i].checkBuffsEndTurn(caster);
        }
    }

    getAllCardData() {
        const res = [];
        for (let i = 0; i < this.cards.length; i++) {
            res.push(this.cards[i].getData());
        }
        return res;
    }

    getModifiers(isAttacker) {
        const res = [];
        let sum = 0;
        for (let i = 0; i < this.cards.length; i ++) {
            const item = this.cards[i];
            if (item.canBattle()) {
                const className = "Card" + item.getId();
                if (CardAbility.hasOwnProperty(className)) {
                    const obj = new CardAbility[className](item);
                    if (CardAbility[className].prototype.hasOwnProperty("getBattleModifier")) {
                        let mod = obj.getBattleModifier(isAttacker);
                        if (mod != 0) {
                            res.push({
                                guid: item.getGuid(),
                                mod: mod
                            });
                            sum += mod;
                        }
                    } else {
                        console.error(`[E]CardSlot.getModifiers: ${className} has not defined getBattleModifier`);
                    }
                }
            }
        }
        return {
            mods: res,
            sum: sum
        };
    }
}

module.exports = CardSlot;