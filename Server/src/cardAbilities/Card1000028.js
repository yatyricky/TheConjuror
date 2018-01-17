const Events = require('../constants/Events');

class Card1000028 {

    constructor(card) {
        this.card = card;
    }

    doAction(player) {
        const cards = player.drawRandom(1, true);
        const deckN = player.getDeck().size();
        return {
            ename: Events.DRAW_CARD,
            payload: {
                name: player.getName(),
                cards: cards,
                deckN: deckN
            }
        };
    }

}

module.exports = Card1000028;