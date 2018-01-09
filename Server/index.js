var app = require('express')();
var server = require('http').Server(app);
var io = require('socket.io')(server);

server.listen(3000);

// Globals

app.get('/', function(req, res) {
    res.send('Hola wourlds!');
});

io.on('connection', function(socket) {
    socket.emit('login_success');

    socket.on('login', function(data) {
        console.log(data.name);
    });
});

console.log('--- Server is running ---');
