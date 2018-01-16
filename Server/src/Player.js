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
        this.cardSlots.push(new CardSlot(Colors.WHITE));
        this.cardSlots.push(new CardSlot(Colors.BLUE));
        this.cardSlots.push(new CardSlot(Colors.ALL));
        this.cardSlots.push(new CardSlot(Colors.GREEN));
        this.cardSlots.push(new CardSlot(Colors.RED));
        this.maxHealth = 20;
        this.health = this.maxHealth;
        this.maxMana = 0;
        this.mana = this.maxMana;
        this.room = null;
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

    checkBuffsEndTurn(caster) {
        for (let i = 0; i < this.cardSlots.length; i ++) {
            this.cardSlots[i].checkBuffsEndTurn(caster);
        }
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

    findCardByGuid(where, guid) {
        let ret = -1;
        for (let i = 0; i < where.length && ret == -1; i++) {
            if (where[i].getGuid() == guid) {
                ret = i;
            }
        }
        if (ret == -1) {
            console.error(`[E]No such card:${guid} in ${JSON.stringify(where)}`);
        }
        return ret;
    }

    playCardFromHand(guid, slotId) {
        const index = this.findCardByGuid(this.hand, guid);
        const card = this.hand[index];
        const res = {
            result: false,
            payloads: []
        };
        if (card.getCost() <= this.mana && this.getRoom().getCurrentPlayer().getName() == this.getName()) {
            let cardDest = -1;
            if (card.getType() == CardTypes.CREATURE || card.getType() == CardTypes.ENCHANTMENT) {
                cardDest = slotId;
                this.cardSlots[slotId].add(card);
            } else if (card.getType() == CardTypes.SPELL) {
                const className = "Card" + card.getId();
                // if (CardAbility.hasOwnProperty(className)) {
                //     const obj = new CardAbility[className](item);
                //     obj.doAction();
                // }
                cardDest = MagicNumbers.TO_GRAVE;
                this.grave.push(card);
            } else {
                console.error(`[E]unknown card type: ${JSON.stringify(card)}`);
            }
            this.hand.splice(index, 1);
            this.mana -= card.getCost();

            res.result = true;
            res.payloads.push({
                ename: "play_card",
                payload: {
                    name: this.getName(),
                    guid: guid,
                    goto: cardDest,
                    mana: this.mana,
                    slotPower: this.getSlotPower(slotId)
                }
            });
        } else {
            res.payloads.push({
                ename: "play_card_fail",
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
                    this.deck.add(new Card(element.id));
                }
            }
        } else {
            console.error("[E]Initing deck with unknown deck name: " + this.deckName);
        }
    }
}

module.exports = Player;