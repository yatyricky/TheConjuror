const Events = require('../constants/Events');
const NoBattle = require('../buff/NoBattle');

class Card1000123 {

    constructor(card) {
        this.card = card;
        this.effectCallback = this.doEffect.bind(this);
    }

    doEffect(player, selection) {
        const ret = [];
        const debuff = new NoBattle(player);
        ret.push(selection.addBuff(debuff));
        ret.push({
            ename: Events.DISCARD_CARD,
            payload: {
                name: player.getName(),
                guid: this.card.getGuid()
            }
        });
        ret.push({
            ename: Events.SELECT_DONE,
            payload: {
                name: player.getName()
            }
        });
        return ret;
    }

    doAction(player) {
        player.pushSelectAction(this.effectCallback);
        return {
            ename: Events.SELECT_TARGET,
            payload: {
                name: player.getName(),
                guid: this.card.getGuid()
            }
        };
    }

}

module.exports = Card1000123;