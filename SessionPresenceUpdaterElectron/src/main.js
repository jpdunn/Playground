/* eslint-disable no-console */

const { app, BrowserWindow, net, ipcMain } = require('electron');
const path = require('path');
const url = require('url');
const fs = require('fs');
const parse = require('parse-duration');
const moment = require('moment');
const DiscordRPC = require('discord-rpc');
const rpc = new DiscordRPC.Client({
    transport: 'ipc'
});


let mainWindow;
let interval;


function createWindow() {
    var width = 700
    var height = 500
    mainWindow = new BrowserWindow({
        width: width,
        height: height,
        resizable: true,
        vibrancy: 'dark',
        hasShadow: false,
        frame: true,
        show: false
    });

    mainWindow.on('ready-to-show', () => {
        mainWindow.show()
    })

    mainWindow.loadURL(url.format({
        pathname: path.join(__dirname, 'index.html'),
        protocol: 'file:',
        slashes: true,
    }));

    mainWindow.webContents.openDevTools();

    mainWindow.on('closed', () => {
        mainWindow = null;
    });
}

app.on('ready', createWindow);

app.on('window-all-closed', () => {
    app.quit();
});

app.on('activate', () => {
    if (mainWindow === null)
        createWindow();
});

app.on('window-all-closed', () => {
    app.quit();
});

app.on('activate', () => {
    if (mainWindow === null)
        createWindow();
});


// Listen for the event that gets fired when the start button is pressed.
ipcMain.on('start-event', (event, args) => {

    console.log(args);
    DiscordRPC.register(args.clientID);
    rpc.login(args.clientID).catch(console.error);

    interval = setInterval(() => {
        setActivity();
    }, 30000);

    console.log('start event called', args);
});

// Listen for the event that gets fired when the stop button is pressed.
ipcMain.on('stop-event', (event, arg) => {
    console.log('stop event called');
    clearInterval(interval);
});


async function setActivity() {
	// Fetch JSON data here.
    const request = net.request('');


    if (!rpc || !mainWindow) {
        return;
    }

    request.on('response', (response) => {

        response.on('data', (chunk) => {
            let data = JSON.parse(chunk);
            console.log(`BODY: ${data}`);
            var activity = {
                details: `${data.pc} Players`,
                state: 'hmm',
                instance: false,
                smallImageKey: 'safe_logo',
                smallImageText: 'Foo',
                largeImageKey: 'mouse_logo',
                largeImageText: 'Hello world'
            };

            rpc.setActivity(activity);
        });
    });

    request.end();
};

rpc.on('ready', () => {
    setActivity();

});
