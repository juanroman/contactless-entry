import socket

class ConnectivityHelper(object):
    def checkConntivityStatus(self):
        try:
            socket.setdefaulttimeout(3)
            socket.socket(socket.AF_INET, socket.SOCK_STREAM).connect(("8.8.8.8", 53))
            return True
        except socket.error as ex:
            print(ex)
            return False
