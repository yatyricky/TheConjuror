const GameData = require('./GameData');
const BuffTypes = require('./constants/BuffTypes');
const Events = require('./constants/Events');

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
        return {
            ename: Events.ADD_BUFF,
            payload: {
                guid: this.guid,
                buff: buff.getData()
            }
        };
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
        for (let i = 0; i < this.buffs.length && can == true; i++) {
            if (this.buffs[i].getType() == BuffTypes.NO_BATTLE) {
                can = false;
            }
        }
        return can;
    }

    takeDamage(num) {
        this.power -= num;
    }

    checkBuffsEndTurn(caster) {
        let ret = [];
        for (let i = 0; i < this.buffs.length; i ++) {
            const item = this.buffs[i];
            if (item.shouldFadeEndTurn() && caster == item.getCaster()) {
                this.removeBuff(item);
                ret = ret.concat([{
                    ename: Events.REMOVE_BUFF,
                    payload: {
                        cguid: this.guid,
                        bguid: item.getData().guid
                    }
                }]);
                i--;
            }
        }
        return ret;
    }

    removeBuff(buff) {
        const index = this.buffs.indexOf(buff);
        this.buffs.splice(index, 1);
    }
}

module.exports = Card;