class Card1000028 {

    constructor(card) {
        this.card = card;
    }

    doAction(player) {
        const cards = player.drawRandom(1, true);
        const deckN = player.getDeck().size();
        return {
            ename: "draw_card",
            payload: {
                name: player.getName(),
                cards: cards,
                deckN: deckN
            }
        };
    }

}

module.exports = Card1000028;