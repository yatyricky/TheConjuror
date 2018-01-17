const Events = require('./src/constants/Events');
const Room = require('./src/Room');
const Player = require('./src/Player');
const GameData = require('./src/GameData');

const app = require('express')();
const server = require('http').Server(app);
const io = require('socket.io')(server);

server.listen(3000);

// Globals
const allRooms = [];
const allClients = {};

io.on('connection', (socket) => {
    socket.emit(Events.CONNECTION_ESTABLISHED);
    console.log('[I][E]New client connected: ' + socket.id);

    socket.on('login', (data) => {
        console.log('[I][O]login:' + JSON.stringify(data));

        // create new player
        let player = new Player(data.name, "blue_basic");
        allClients[data.name] = player;
        socket.playerName = data.name;

        let room = null;
        for (let i = 0; i < allRooms.length && room == null; i++) {
            if (allRooms[i].canJoin()) {
                room = allRooms[i];
            }
        }
        if (room == null) {
            // No room found
            room = new Room();
            allRooms.push(room);
            console.log('[I]Created new room');
        }
        const stillEmpty = room.playerJoin(player);
        socket.join(room.getId());
        if (stillEmpty) {
            socket.emit(Events.LOGIN, {
                cmd: 'wait'
            });
            console.log('[I][E]login: wait');
        } else {
            const playerSocketIds = Object.keys(io.sockets.adapter.rooms[room.getId()].sockets);
            if (playerSocketIds.length == 2) {
                const payloads = room.start();
                const p1 = room.p1();
                const p2 = room.p2();
                io.to(room.getId()).emit(Events.LOGIN, {
                    cmd: 'start',
                    players: {
                        p1: p1.getUIData(),
                        p2: p2.getUIData()
                    }
                });
                console.log('[I][B]2 players login ui data');

                for (let i = 0; i < payloads.length; i++) {
                    const element = payloads[i];
                    io.to(room.getId()).emit(element.ename, element.payload);
                    console.log(`[I][B]${element.ename}: ${JSON.stringify(element.payload)}`);
                }
                console.log('[I][B]Game start');
            } else {
                console.error('[E]Game about to start, but there are not exact 2 players');
            }
        }
    });

    socket.on('disconnect', (reason) => {
        const player = allClients[socket.playerName];
        let removed = 0;
        for (let i = 0; i < allRooms.length; i++) {
            removed += allRooms[i].playerLeave(player);
        }
        delete allClients[socket.playerName];
        if (removed != 1) {
            console.error('[E]Removing player from rooms, instances: ' + removed);
        }
        console.log(`[I][O]Client disconnected: ${socket.id}, reason: ${JSON.stringify(reason)}`);
    });

    socket.on('play_card', (data) => {
        console.log(`[I][O]play_card: ${JSON.stringify(data)}`);
        const player = allClients[data.name];
        const res = player.playCardFromHand(data.guid, data.slotId);
        if (res.result === true) {
            for (let i = 0; i < res.payloads.length; i++) {
                const element = res.payloads[i];
                io.to(player.getRoom().getId()).emit(element.ename, element.payload);
                console.log(`[I][B]${element.ename}: ${JSON.stringify(element.payload)}`);   
            }
        } else {
            for (let i = 0; i < res.payloads.length; i++) {
                const element = res.payloads[i];
                socket.emit(element.ename, element.payload);
                console.log(`[I][E]${element.ename}: ${JSON.stringify(element.payload)}`);   
            }
        }
    });

    socket.on('end_turn', (data) => {
        console.log(`[I][O]end_turn: ${JSON.stringify(data)}`);
        const player = allClients[data.name];
        const room = player.getRoom();
        const payload = room.endTurnBy(player);
        if (payload.success) {
            for (let i = 0; i < payload.endTurnResps.length; i++) {
                const element = payload.endTurnResps[i];
                io.to(room.getId()).emit(element.ename, element.payload);
                console.log(`[I][B]${element.ename}: ${JSON.stringify(element.payload)}`);
            }
        }
    });

    socket.on('attack_slot', (data) => {
        console.log(`[I][O]attack_slot: ${JSON.stringify(data)}`);
        const player = allClients[data.name];
        const room = player.getRoom();
        const payloads = room.playerAttackSlot(player, data.from, data.to);
        for (let i = 0; i < payloads.length; i++) {
            const element = payloads[i];
            io.to(room.getId()).emit(element.ename, element.payload);
            console.log(`[I][B]${element.ename}: ${JSON.stringify(element.payload)}`);
        }
    });

});

console.log('--- Server is running ---');
