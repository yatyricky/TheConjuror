const Events = {
    CONNECTION_ESTABLISHED: "connection_established",
    LOGIN: "login",
    PLAY_CARD: "play_card",
    PLAY_CARD_FAIL: "play_card_fail",
    PLAY_CARD_SLOT: "play_card_slot",
    DISCARD_CARD: "discard_card",
    UPDATE_MANA: "update_mana",
    DRAW_CARD: "draw_card",
    TURN_FOR: "turn_for",
    BATTLE_RES: "battle_res",
    SELECT_TARGET: "select_target",
    SELECT_SLOT: "select_slot",
    SELECT_DONE: "select_done",
    ADD_BUFF: "add_buff",
    REMOVE_BUFF: "remove_buff",
    UPDATE_SLOT_POWER: "update_slot_power",
    UPDATE_CARD_POWER: "update_card_power"
};

module.exports = Events;