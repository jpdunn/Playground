const { webFrame, remote, ipcRenderer } = require('electron');
const mainProcess = remote.require('./main.js');
const os = require('os');


if (os.type() !== 'Darwin') {
  document.body.style.backgroundColor = '#4C4C4C'
}

let clientID;
let sessionName;
let smallImageKey;
let smallImageText;
let largeImageKey;
let largeImageText;


document.getElementById('stopButton').addEventListener('click', onStopClicked);
document.getElementById('startButton').addEventListener('click', onStartClicked);

webFrame.setZoomLevelLimits(1, 1);

function onStartClicked() {
  clientID = document.getElementById('clientIDInput').value;
  sessionName = document.getElementById('sessionNameInput').value;
  smallImageKey = document.getElementById('smallImageKeyInput').value;
  smallImageText = document.getElementById('smallImageTextInput').value;
  largeImageKey = document.getElementById('largeImageKeyInput').value;
  largeImageText = document.getElementById('largeImageTextInput').value;

  // TODO: Instead of this, only activate the buttons once the correct input has been given.
  if (!sessionName || sessionName === "") {
    alert('Please enter a session name');
    return;
  }

  if (!clientID || clientID === "") {
    alert('Please enter a client ID');
    return;
  }

  // Only do stuff if there is a client ID.
  ipcRenderer.send('start-event', {
    clientID,
    sessionName,
    smallImageKey,
    smallImageText,
    largeImageKey,
    largeImageText
  });

};

function onStopClicked() {
  ipcRenderer.send('stop-event');
};


