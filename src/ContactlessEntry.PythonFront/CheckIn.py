from __future__ import print_function
from connectivity import ConnectivityHelper
import app_constants
from tkinter import *
from tkinter import ttk
from tkinter import messagebox
import time

class CheckInApplication:
    def __init__(self):
        print("[DEBUG] CheckInWidget Constructor...")
        
        self.initializeWidgets()
        self.checkInternetStatus()
        #ch = ConnectivityHelper()
        #connected = ch.checkConntivityStatus()
        #if (connected):
        #    self.connectivityLabel.configure("text", "Online")
        #else:
        #    self.connectivityLabel.configure("text", "Offline")
        
    
    def initializeWidgets(self):
        self.window = Tk()
        self.window.geometry("600x600")
        self.window.title(app_constants.APP_NAME)
        self.window.wm_protocol("WM_DELETE_WINDOW", self.closing)
        Grid.rowconfigure(self.window, 0, weight=1)
        Grid.columnconfigure(self.window, 0, weight=1)
        
        self.master = Frame(self.window, padx=5, pady=5)
        self.master.grid(row=0, column=0, sticky=N+S+E+W)
        self.master.columnconfigure(1, weight=1)
        self.master.rowconfigure(1, weight=1)

        self.connectivityLabel = Label(self.master,
                                       text="Online")
        self.connectivityLabel.grid(row=0, column=0)

        self.dateTimeLabel = Label(self.master)
        self.dateTimeLabel.grid(row=0, column=2)
        
        self.weatherLabel = Label(self.master,
                                  text="21°")
        self.weatherLabel.grid(row=2, column=0)

        self.quitButton = Button(self.master,
                                 text="Quit",
                                 command=self.onClose)
        self.quitButton.grid(row=2, column=2)


    def onClose(self):
        answer = messagebox.askyesno(app_constants.APP_NAME, "¿Estas seguro que deseas salir?")
        if answer:
            self.closing()

    def closing(self):
        print("[DEBUG] closing...")
        self.window.quit()

    def checkInternetStatus(self):
        now = time.strftime("%a %d %b   %H:%M")
        self.dateTimeLabel.configure(text=now)
        self.window.after(1000, self.checkInternetStatus)
