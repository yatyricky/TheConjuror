# Listening

## login

``` js
{
    name: "player name"
}
```

## play_card

``` js
{
    name: "player name",
    guid: "card guid",
    slotId: "which slot"
}
```

## end_turn

``` js
{
    name: "player name"
}
```

## select_slot

``` js
{
    name: "player name",
    target: "target player name",
    slot: "slot id"
}
```

## attack_slot

``` js
{
    name: "player name",
    from: "attacking slot",
    to: "defending slot"
}
```

# Emit

## connection_established

``` js
{}
```

## login

Case 1: when player needs to wait for another player.

``` js
{
    cmd: 'wait'
}
```

Case 2: game start.

``` js
{
    cmd: 'start',
    players: {
        p1: {
            name: "player name",
            hp: "player hp",
            mp: "player mana"
        },
        p2: {
            name: "player name",
            hp: "player hp",
            mp: "player mana"
        }
    }
}
```

## play_card

``` js
{
    name: "player name",
    guid: "card guid",
    mana: "player name"
}
```

## play_card_fail

``` js
{
    name: "player name",
    guid: "card guid"
}
```

## update_mana

``` js
{
    name: "player name",
    mana: "player mana"
}
```

## draw_card

``` js
{
    name: "player name",
    cards: [{
        guid: "card guid",
        id: "card id",
        name: "card name",
        color: "card color",
        ctype: "card type",
        power: "card power",
        desc: "card description",
        cost: "card cost"
    }],
    deckN: "player deck remaining cards"
}
```

## turn_for

``` js
{
    name: "the acting player"
}
```

## battle_res

``` js
{
    res: {
        amods: [{ // attacker cards modifiers
            guid: "card guid",
            mod: "card battle power modifier"
        }],
        dmods: [{ // defender ~
            guid: "card guid",
            mod: "card battle power modifier"
        }],
        aoap: "attacker original power",
        doap: "defender ~",
        abattle: [{ /* attacker battling cards */ }],
        bbattle: [{ /* defender ~ */ }],
        aaap: "attacker after battle power",
        daap: "defender ~",
        akilled: [{ /* attacker battle killed cards */ }],
        dkilled: [{ /* defender ~ */ }],
        atouched: [{ /* attacker battle damaged cards */ }],
        dtouched: [{ /* defender ~ */ }],
        ahit: "attacker taken damage",
        dhit: "defender ~",
        aname: "attacker name",
        dname: "defender ~",
        acs: "attacker card slot",
        dcs: "defender ~",
        ahp: "attacker after battle health",
        dhp: "defender ~"
    }
}
```

## select_target

``` js
{
    name: "player name",
    guid: "card guid"
}
```

## select_slot

``` js
{
    name: "player name",
    guid: "card guid"
}
```

## play_card_slot

``` js
{
    name: "player name",
    guid: "card guid",
    slot: "slot id",
    power: "slot power"
}
```

## discard_card

``` js
{
    name: "player name",
    guid: "card guid"
}
```

## select_done

``` js
{
    name: "player name"
}
```

## add_buff

``` js
{
    guid: "card guid",
    buff: {
        guid: "buff guid",
        icon: "buff icon"
    }
}
```

## remove_buff

``` js
{
    cguid: "card guid",
    bguid: "buff guid"
}
```

## update_slot_power

``` js
{
    name: "player name",
    slot: "which slot",
    power: "power value"
}
```

## update_card_power

``` js
{
    guid: "card guid",
    power: "power value"
}
```