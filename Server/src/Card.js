const GameData = require('./GameData');

let guid = 0;

class Card {

    constructor(id) {
        if (GameData.cardBase.hasOwnProperty(id)) {
            const cardData = GameData.cardBase[id];
            this.id = id;
            this.name = cardData.name;
            this.color = cardData.color;
            this.ctype = cardData.ctype;
            this.cost = cardData.cost;
            this.description = cardData.description;
            this.power = cardData.power;
            this.maxPower = this.power;
            this.buffs = [];
            this.guid = guid++;
        } else {
            console.error("[E]Creating card from unknown id: " + id);
        }
    }

    addBuff(buff) {
        this.buffs.push(buff);
    }

    getGuid() {
        return this.guid;
    }

    getId() {
        return this.id;
    }

    getPower() {
        return this.power;
    }

    getData() {
        return {
            guid: this.guid,
            id: this.id,
            name: this.name,
            color: this.color,
            ctype: this.ctype,
            power: this.power,
            desc: this.description,
            cost: this.cost
        };
    }

    getCost() {
        return this.cost;
    }

    getType() {
        return this.ctype;
    }

    canBattle() {
        let can = true;
        for (let i = 0; i < this.buffs.length; i++) {
            // TODO
            // const element = this.buffs[i];
            // if (element.Type == Buff.BuffType.NO_BATTLE) {
            //     can = false;
            // }
        }
        return can;
    }

    takeDamage(num) {
        this.power -= num;
    }

    checkBuffsEndTurn(caster) {
        for (let i = 0; i < this.buffs.length; i ++) {
            // TODO
            // const item = this.buffs[i];
            // if (item.ShouldRemoveEndTurn() && caster == item.Caster) {
            //     RemoveBuff(item);
            //     i--;
            // }
        }
    }

    removeBuff(buff) {
        // TODO
        // this.buffs.Remove(buff);
        // COB.RemoveBuff(buff.BO);
    }
}

module.exports = Card;