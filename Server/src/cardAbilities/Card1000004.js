const Events = require('../constants/Events');
const Weaken = require('../buff/Weaken');

class Card1000004 {

    constructor(card) {
        this.card = card;
        this.effectCallback = this.doEffect.bind(this);
    }

    doEffect(player, target, selection) {
        const ret = [];
        const cards = selection.getCards();
        for (let i = 0; i < cards.length; i++) {
            const debuff = new Weaken(player);
            ret.push(cards[i].addBuff(debuff));
            // update card power
            ret.push({
                ename: Events.UPDATE_CARD_POWER,
                payload: {
                    guid: cards[i].getData().guid,
                    power: cards[i].getData().power
                }
            });
        }

        // update slot power
        ret.push({
            ename: Events.UPDATE_SLOT_POWER,
            payload: {
                name: target.getName(),
                slot: selection.getId(),
                power: selection.getTotalPower()
            }
        });

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
            ename: Events.SELECT_SLOT,
            payload: {
                name: player.getName(),
                guid: this.card.getGuid()
            }
        };
    }

}

module.exports = Card1000004;