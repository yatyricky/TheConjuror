const GameData = require('./GameData');
const BuffTypes = require('./constants/BuffTypes');
const Events = require('./constants/Events');

let guid = 0;

class Card {

    constructor(id, player) {
        if (GameData.cardBase.hasOwnProperty(id)) {
            const cardData = GameData.cardBase[id];
            this.id = id;
            this.owner = player;
            this.name = cardData.name;
            this.color = cardData.color;
            this.ctype = cardData.ctype;
            this.cost = cardData.cost;
            this.description = cardData.description;
            this.power = cardData.power;
            this.maxPower = this.power;
            this.buffs = [];
            this.attrs = {};
            this.guid = guid++;
        } else {
            console.error("[E]Creating card from unknown id: " + id);
        }
    }

    getOwner() {
        return this.owner;
    }

    addBuff(buff) {
        this.buffs.push(buff);
        buff.onApply(this);
        return {
            ename: Events.ADD_BUFF,
            payload: {
                guid: this.guid,
                buff: buff.getData()
            }
        };
    }

    getAttrs() {
        return this.attrs;
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

    modPower(mod) {
        this.power += mod;
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
        if (!this.attrs.hasOwnProperty(BuffTypes.NoBattle)) {
            this.attrs[BuffTypes.NoBattle] = 0;
        }
        return this.attrs[BuffTypes.NoBattle] == 0;
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
                }, {
                    ename: Events.UPDATE_CARD_POWER,
                    payload: {
                        guid: this.guid,
                        power: this.power
                    }
                }]);
                i--;
            }
        }
        return ret;
    }

    removeBuff(buff) {
        buff.onRemove(this);
        const index = this.buffs.indexOf(buff);
        this.buffs.splice(index, 1);
    }
}

module.exports = Card;