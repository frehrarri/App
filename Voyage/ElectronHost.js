/// <reference path="preload.js" />
const { app, BrowserWindow, ipcMain } = require('electron');
const { autoUpdater } = require('electron-updater');
const path = require('path');
const { spawn } = require('child_process');
const fs = require('fs');

let mainWindow;
let dotnetProcess;

// Allow HTTPS localhost with self-signed certs
app.commandLine.appendSwitch("allow-insecure-localhost", "true");
app.commandLine.appendSwitch("ignore-certificate-errors", "true");

// Preload path (root in dev, resourcesPath in packaged)
//dev meaning webapp, packaged meaninig downloaded offline executable
const preloadPath = app.isPackaged
    ? path.join(process.resourcesPath, "preload.js")
    : path.join(__dirname, "preload.js");
console.log("Preload path:", preloadPath, "Exists:", fs.existsSync(preloadPath));


function startDotNetApp() {
    return new Promise((resolve) => {
        if (!app.isPackaged) {
            console.log("Dev mode: skipping .NET spawn (run dotnet run manually)");
            return resolve();
        }

        const dllPath = path.join(process.resourcesPath, "published-app", "Voyage.dll");
        const dotnetExe = "C:\\Program Files\\dotnet\\dotnet.exe";

        console.log("Starting .NET from:", dllPath);

        dotnetProcess = spawn(dotnetExe, [dllPath], {
            cwd: path.dirname(dllPath)
        });

        dotnetProcess.stdout.on("data", (data) =>
            console.log("[.NET]", data.toString())
        );

        dotnetProcess.stderr.on("data", (data) =>
            console.error("[.NET ERROR]", data.toString())
        );

        // Give .NET a few seconds to start
        setTimeout(resolve, 3000);

    });
}

async function createWindow() {
    await startDotNetApp();

    mainWindow = new BrowserWindow({
        width: 1200,
        height: 800,
        frame: false,
        webPreferences: {
            preload: preloadPath,
            contextIsolation: true,
            nodeIntegration: false,
            sandbox: true,                    
            webSecurity: true,                
            allowRunningInsecureContent: false,
            devTools: false
        },
    });

    // Open DevTools automatically in dev
    if (!app.isPackaged) {
        mainWindow.webContents.openDevTools({ mode: "detach" });
    }

    // Load your MVC app
    const startupUrl = "https://localhost:7000/PWA/Login";
    console.log("Loading URL:", startupUrl);
    mainWindow.loadURL(startupUrl);


    mainWindow.on('closed', () => {
        mainWindow = null;
    });

    // Check for updates in executable
    if (app.isPackaged) {
        autoUpdater.checkForUpdatesAndNotify();
    }
}

// Auto-updater events
autoUpdater.on('update-available', () => {
    mainWindow.webContents.send('update-available');
});

autoUpdater.on('update-downloaded', () => {
    mainWindow.webContents.send('update-downloaded');
});

// Handle window controls
ipcMain.on('window-minimize', () => {
    if (mainWindow) mainWindow.minimize();
});

ipcMain.on('window-maximize', () => {
    if (mainWindow) {
        if (mainWindow.isMaximized()) {
            mainWindow.unmaximize();
        } else {
            mainWindow.maximize();
        }
    }
});

ipcMain.on('window-close', () => {
    if (mainWindow) mainWindow.close();
});

// ====================
// App Lifecycle
// ====================

app.whenReady().then(createWindow);

app.on('window-all-closed', () => {
    // Kill the .NET process
    if (dotnetProcess) {
        dotnetProcess.kill();
    }
    if (process.platform !== 'darwin') {
        app.quit();
    }
});

app.on('before-quit', () => {
    if (dotnetProcess) {
        dotnetProcess.kill();
    }
});

app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) {
        createWindow();
    }
});