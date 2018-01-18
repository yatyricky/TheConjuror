const Events = require('./constants/Events');
const Colors = require('./constants/Colors');
const CardTypes = require('./constants/CardTypes');
const MagicNumbers = require('./constants/MagicNumbers');
const CardSlot = require('./CardSlot');
const Card = require('./Card');
const Deck = require('./Deck');
const GameData = require('./GameData');
const CardAbility = require('./cardAbilities/CardAbility');

class Player {

    constructor(name, deckName) {
        this.name = name;
        this.deckName = deckName;
        this.deck = null;
        this.hand = [];
        this.grave = [];
        this.cardSlots = []; // 5 max
        this.cardSlots.push(new CardSlot(Colors.WHITE, 0, this));
        this.cardSlots.push(new CardSlot(Colors.BLUE, 1, this));
        this.cardSlots.push(new CardSlot(Colors.ALL, 2, this));
        this.cardSlots.push(new CardSlot(Colors.GREEN, 3, this));
        this.cardSlots.push(new CardSlot(Colors.RED, 4, this));
        this.maxHealth = 20;
        this.health = this.maxHealth;
        this.maxMana = 0;
        this.mana = this.maxMana;
        this.room = null;
        this.selectActionStack = [];
    }

    getName() {
        return this.name;
    }

    getRoom() {
        return this.room;
    }

    getUIData() {
        return {
            name: this.name,
            hp: this.health,
            mp: this.mana
        }
    }

    pushSelectAction(callback) {
        this.selectActionStack.push(callback);
    }

    checkBuffsEndTurn(caster) {
        let ret = [];
        for (let i = 0; i < this.cardSlots.length; i ++) {
            ret = ret.concat(this.cardSlots[i].checkBuffsEndTurn(caster));
        }
        for (let i = 0; i < this.grave.length; i ++) {
            ret = ret.concat(this.grave[i].checkBuffsEndTurn(caster));
        }
        return ret;
    }

    restoreSlotAttackCharges() {
        for (let i = 0; i < this.cardSlots.length; i++) {
            this.cardSlots[i].restoreSlotAttackCharges();
        }
    }

    getDeck() {
        return this.deck;
    }

    attack(defender, acs, dcs) {
        let res = {};

        // reduce slot attack charges by 1
        this.cardSlots[acs].consumeCharge();
        // On Attack
        // modifiers: [{guid, mod}]
        let ares = this.cardSlots[acs].getModifiers(true);
        let dres = defender.cardSlots[dcs].getModifiers(false);
        res.amods = ares.mods; // modifiers
        res.dmods = dres.mods;
        res.aoap = this.getSlotPower(acs); // original attack power
        res.doap = defender.getSlotPower(dcs);
        let attackerPower = res.aoap + ares.sum;
        let defenderPower = res.doap + dres.sum;
        res.abattle = this.cardSlots[acs].getAllCardData();
        res.bbattle = defender.cardSlots[dcs].getAllCardData();

        // Slots battle
        let admgRes = this.cardSlots[acs].takeDamage(defenderPower);
        let ddmgRes = defender.cardSlots[dcs].takeDamage(attackerPower);
        this.grave = this.grave.concat(admgRes.killed);
        defender.grave = defender.grave.concat(ddmgRes.killed);
        res.aaap = this.getSlotPower(acs); // attacker after attack power
        res.daap = defender.getSlotPower(dcs);

        res.akilled = [];
        for (let i = 0; i < admgRes.killed.length; i++) {
            const element = admgRes.killed[i];
            this.grave.push(element);
            res.akilled.push(element.getData());
        }
        res.atouched = [];
        for (let i = 0; i < admgRes.touched.length; i++) {
            res.atouched.push(admgRes.touched[i]);
        }
        res.dkilled = [];
        for (let i = 0; i < ddmgRes.killed.length; i++) {
            const element = ddmgRes.killed[i];
            defender.grave.push(element);
            res.dkilled.push(element.getData());
        }
        res.dtouched = [];
        for (let i = 0; i < ddmgRes.touched.length; i++) {
            res.dtouched.push(ddmgRes.touched[i]);
        }
        res.ahit = 0;
        res.dhit = 0;
        res.aname = this.name;
        res.dname = defender.name;
        res.acs = acs;
        res.dcs = dcs;

        // inflict damage to player
        if (attackerPower > defenderPower) {
            defender.takeDamage(1);
            res.dhit = 1;
        } else if (attackerPower < defenderPower) {
            this.takeDamage(1);
            res.ahit = 1;
        }
        res.ahp = this.health;
        res.dhp = defender.health;
        return res;
    }

    takeDamage(v) {
        this.health -= v;
    }

    canAttackWithSlot(slotId) {
        return this.getSlotPower(slotId) > 0 && this.cardSlots[slotId].getAttackCharges() > 0;
    }

    canPlayCardToSlot(cardData, slotId) {
        return cardData.getCost() <= this.mana;
    }

    getMana() {
        return this.mana;
    }

    turnAddMana() {
        this.maxMana = this.room.getCurrentTurn();
        if (this.maxMana > MagicNumbers.MAX_MANA) {
            this.maxMana = MAX_MANA;
        }
        this.mana = this.maxMana;
    }

    joinRoom(room) {
        this.room = room;
    }

    getSlotPower(slotId) {
        return this.cardSlots[slotId].getTotalPower();
    }

    drawRandom(num, cardDataOnly = false) {
        const ret = [];
        for (let i = 0; i < num; i++) {
            const card = this.deck.drawRandom();
            this.hand.push(card);
            if (cardDataOnly === true) {
                ret.push(card.getData());
            } else {
                ret.push(card);
            }
        }
        return ret;
    }

    findCardByGuid(guid, warn = true, slots = false, hand = false, grave = false) {
        let found = -1;
        let where = null;
        if (slots === true) {
            for (let i = 0; i < this.cardSlots.length && found == -1; i++) {
                let cards = this.cardSlots[i].getCards();
                for (let j = 0; j < cards.length && found == -1; j++) {
                    if (cards[j].getGuid() == guid) {
                        found = j;
                        where = cards;
                    }
                }
            }
        }
        if (hand === true && found == -1) {
            for (let i = 0; i < this.hand.length && found == -1; i++) {
                if (this.hand[i].getGuid() == guid) {
                    found = i;
                    where = this.hand;
                }
            }
        }
        if (grave === true && found == -1) {
            for (let i = 0; i < this.grave.length && found == -1; i++) {
                if (this.grave[i].getGuid() == guid) {
                    found = i;
                    where = this.grave;
                }
            }
        }
        if (found == -1) {
            if (warn === true) {
                console.error(`[E]No such card:${guid} in ${this.name}`);
            }
            return null;
        } else {
            return {
                where: where,
                index: found
            };
        }
    }

    locateCard(card) {
        let ret = -1;
        for (let i = 0; i < this.cardSlots.length && ret == -1; i++) {
            const index = this.cardSlots[i].getCards().indexOf(card);
            if (index != -1) {
                ret = i;
            }
        }
        if (ret == -1) {
            console.error(`[E]Player.locateCard: unable to find card:${card} in ${this.name}`);
        }
        return ret;
    }

    selectedCardByGuid(guid) {
        const card = this.room.findCardByGuid(guid, true, false, false);
        if (this.selectActionStack.length <= 0) {
            console.error(`[E]No select action pushed`);
        } else {
            const act = this.selectActionStack.pop();
            return act(this, card);
        }
    }

    selectedSlot(target, sid) {
        if (this.selectActionStack.length <= 0) {
            console.error(`[E]No select action pushed`);
        } else {
            const act = this.selectActionStack.pop();
            return act(this, target, target.cardSlots[sid]);
        }
    }

    playCardFromHand(guid, slotId) {
        const indexBundle = this.findCardByGuid(guid, true, false, true, false);
        const index = indexBundle.index;
        const card = this.hand[index];
        const res = {
            result: false,
            payloads: []
        };
        if (card.getCost() <= this.mana && this.getRoom().getCurrentPlayer().getName() == this.getName()) {
            this.hand.splice(index, 1);
            this.mana -= card.getCost();
            res.result = true;
            res.payloads.push({
                ename: Events.PLAY_CARD,
                payload: {
                    name: this.getName(),
                    guid: guid,
                    mana: this.mana
                }
            });

            if (card.getType() == CardTypes.CREATURE || card.getType() == CardTypes.ENCHANTMENT) {
                this.cardSlots[slotId].add(card);
                res.payloads.push({
                    ename: Events.PLAY_CARD_SLOT,
                    payload: {
                        name: this.getName(),
                        guid: guid,
                        slot: slotId,
                        power: this.getSlotPower(slotId)
                    }
                });
            } else if (card.getType() == CardTypes.SPELL) {
                const className = "Card" + card.getId();
                if (CardAbility.hasOwnProperty(className)) {
                    const obj = new CardAbility[className](card);
                    if (CardAbility[className].prototype.hasOwnProperty("doAction")) {
                        const resp = obj.doAction(this);
                        if (resp !== undefined && resp.hasOwnProperty("ename")) {
                            res.payloads.push(resp);
                            if (resp.ename == Events.SELECT_TARGET || resp.ename == Events.SELECT_SLOT) {
                                // Target spells
                            } else {
                                // Normal spells
                                this.grave.push(card);
                                res.payloads.push({
                                    ename: Events.DISCARD_CARD,
                                    payload: {
                                        name: this.getName(),
                                        guid: guid
                                    }
                                });
                            }
                        }
                    } else {
                        console.error(`[E]Player.playCardFromHand: ${className} has not defined doAction`);
                    }
                } else {
                    console.error(`[E]Player.playCardFromHand: Card${card.getId()}.js doesnt exist`);
                }
            } else {
                console.error(`[E]unknown card type: ${JSON.stringify(card)}`);
            }
        } else {
            res.payloads.push({
                ename: Events.PLAY_CARD_FAIL,
                payload: {
                    name: this.getName(),
                    guid: guid
                }
            });
        }
        return res;
    }

    initDeck() {
        if (GameData.deckBase.hasOwnProperty(this.deckName)) {
            this.deck = new Deck();
            for (let i = 0; i < GameData.deckBase[this.deckName].length; i++) {
                const element = GameData.deckBase[this.deckName][i];
                for (let j = 0; j < element.num; j ++) {
                    this.deck.add(new Card(element.id, this));
                }
            }
        } else {
            console.error("[E]Initing deck with unknown deck name: " + this.deckName);
        }
    }
}

module.exports = Player;