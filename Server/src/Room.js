const MagicNumbers = require('./constants/MagicNumbers');
const Events = require('./constants/Events');

class Room {

    constructor() {
        this.id = Date.now();
        this.init();
        this.turnStartEvents = [];
        this.turnEndEvents = [];
        this.registerTurnStartEvent(this.endTurn.bind(this));
        this.registerTurnStartEvent(this.restoreManaAndAddOne.bind(this));
        this.registerTurnStartEvent(this.playerDrawCard.bind(this));
    }

    init() {
        this.player1 = null;
        this.player2 = null;
        this.currentPlayer = null;
        this.currentTurn = -1;
    }

    getId() {
        return this.id;
    }

    p1() {
        return this.player1;
    }

    p2() {
        return this.player2;
    }

    canJoin() {
        return this.player1 == null || this.player2 == null;
    }

    isEmpty() {
        return this.player1 == null && this.player2 == null;
    }

    playerJoin(player) {
        if (this.player1 == null) {
            this.player1 = player;
        } else if (this.player2 == null) {
            this.player2 = player;
        } else {
            console.error('[E][Room]: Joining player into full room');
        }
        player.joinRoom(this);
        return this.canJoin();
    }

    getCurrentPlayer() {
        return this.currentPlayer;
    }

    getCurrentTurn() {
        let c = this.currentTurn;
        if (c < 0) {
            c = 0;
        }
        return Math.floor(c / 2.0) + 1;
    }

    playerLeave(player) {
        let removed = 0;
        if (this.player1 == player) {
            this.player1 = null;
            removed += 1;
        }
        if (this.player2 == player) {
            this.player2 = null;
            removed += 1;
        }
        if (this.isEmpty()) {
            this.init();
        }
        return removed;
    }

    start() {
        this.currentPlayer = this.player1;
        this.player1.initDeck();
        this.player2.initDeck();
        return this.triggerEvents(this.turnStartEvents);
    }

    endTurnBy(player) {
        const payload = {
            success: false,
            endTurnResps: []
        };
        if (this.getCurrentPlayer() == player) {
            // you have the right to do this
            payload.success = true;
            payload.endTurnResps = payload.endTurnResps.concat(this.triggerEvents(this.turnEndEvents));
            payload.endTurnResps = payload.endTurnResps.concat(this.triggerEvents(this.turnStartEvents));
        }
        return payload;
    }

    restoreManaAndAddOne() {
        this.currentPlayer.turnAddMana();
        return [{
            ename: Events.UPDATE_MANA,
            payload: {
                name: this.currentPlayer.getName(),
                mana: this.currentPlayer.getMana()
            }
        }];
    }

    playerDrawCard() {
        const ret = [];
        if (this.currentTurn > 0) {
            // Every normal turn
            const cardsData = this.currentPlayer.drawRandom(1, true);
            const deckSize = this.currentPlayer.getDeck().size();
            ret.push({
                ename: Events.DRAW_CARD,
                payload: {
                    name: this.currentPlayer.getName(),
                    cards: cardsData,
                    deckN: deckSize
                }
            });
        } else {
            // First turn
            let num = MagicNumbers.FIRST_DRAW;
            if (this.player1 == this.currentPlayer) {
                num += MagicNumbers.EACH_DRAW;
            }
            let cardsData = this.player1.drawRandom(num, true);
            let deckSize = this.player1.getDeck().size();
            ret.push({
                ename: Events.DRAW_CARD,
                payload: {
                    name: this.player1.getName(),
                    cards: cardsData,
                    deckN: deckSize
                }
            });
            num = MagicNumbers.FIRST_DRAW;
            if (this.player2 == this.currentPlayer) {
                num += MagicNumbers.EACH_DRAW;
            }
            cardsData = this.player2.drawRandom(num, true);
            deckSize = this.player2.getDeck().size();
            ret.push({
                ename: Events.DRAW_CARD,
                payload: {
                    name: this.player2.getName(),
                    cards: cardsData,
                    deckN: deckSize
                }
            });
        }
        return ret;
    }

    endTurn() {
        this.currentTurn += 1;
        if (this.currentTurn % 2 == 0) {
            this.currentPlayer = this.player1;
        } else {
            this.currentPlayer = this.player2;
        }
        this.currentPlayer.restoreSlotAttackCharges();
        return [{
            ename: Events.TURN_FOR,
            payload: {
                name: this.currentPlayer.getName()
            }
        }];
    }

    playerAttackSlot(player, from, to) {
        if (player == this.currentPlayer) {
            if (player.canAttackWithSlot(from)) {
                return [{
                    ename: Events.BATTLE_RES,
                    payload: {
                        res: player.attack(this.getOpponent(player), from, to)
                    }
                }];
            } else {
                console.log(`[I]player ${player.getName()} cant attack`);
                return [];
            }
        } else {
            console.log(`[E]Room.playerAttackSlot: player=${player.getName()} curr=${this.currentPlayer.getName()}`);
            return [];
        }
    }

    getOpponent(player) {
        if (player == this.player1) {
            return this.player2;
        } else if (player == this.player2) {
            return this.player1;
        } else {
            console.error(`[E]Get opponent of non-existing player in the room p:${player.getName()}, p1:${this.player1.getName()}, p2:${this.player2.getName()}`);
            return null;
        }
    }

    triggerEvents(whichEvent) {
        let results = [];
        for (let i = 0; i < whichEvent.length; i++) {
            results = results.concat(whichEvent[i]());
        }
        return results;
    }
    
    registerTurnStartEvent(callback) {
        this.turnStartEvents.push(callback);
    }

    registerTurnEndEvent(callback) {
        this.turnEndEvents.push(callback);
    }
}

module.exports = Room;