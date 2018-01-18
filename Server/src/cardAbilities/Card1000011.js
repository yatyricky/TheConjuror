const Events = require('../constants/Events');

class Card1000011 {

    constructor(card) {
        this.card = card;
    }

    onEnterGrave(player) {
        const cardData = player.drawRandom(1, true);
        return {
            ename: Events.DRAW_CARD,
            payload: {
                name: player.getName(),
                cards: cardData,
                deckN: player.getDeck().size()
            }
        };
    }

}

module.exports = Card1000011;