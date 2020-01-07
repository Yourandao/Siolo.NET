import requests
import base64
import json
from os import path

SERVICE_IP = 'localhost'
SERVICE_PORT = 51343

current_session = ''
opened_sessions = []

def sendToService(data: dict, cmd_addr: str, files: dict = None):
    url = f'http://{SERVICE_IP}:{SERVICE_PORT}/{cmd_addr}/'
    if files is None:
        r = requests.post(url, json=data)
    else:
        r = requests.post(url, data=data, files=files)
    return json.loads(r.text)['message']



def isSessionUsed():
    if current_session == '':
        print('You are not using any sessions (use <ip>)')
        return False
    return True


def printHello():
    print('<Siolo client>')
    print('Print help or any command')


def cmdHelp():
    print('Available commands:')
    print('help                 -   print help information')
    print('show <what>          -   show smth (sessions)')
    print('forceuse <ip>        -   use not opened session in current client')
    print('use <ip>             -   use opened session in current client')
    print('login <ip>           -   login ip to system and open session (it will register ip for first time)')
    print('forcelogout <ip>     -   force logut ip from system')
    print('logout <ip/all/cur>  -   logut ip from system')
    print()
    print('In session commands:')
    print('background           -   background current session')
    print('drop <file_path>     -   trigger drop file event')
    print()
    print('exit                 -   disconnect all sessions and leave script')
    print()


def cmdShow(what: str):
    if what == 'sessions':
        for i, s in zip(range(len(opened_sessions)), opened_sessions):
            print(f'{i}: {s}')
        return

    print(f'Can\'t show "{what}"')


def cmdForceUse(ip: str):
    global current_session
    current_session = ip


def cmdUse(ip: str):
    if ip not in opened_sessions:
        print('This session is not opened on this client')
        return
    cmdForceUse(ip)


def cmdLogin(ip: str):
    if ip in opened_sessions:
        print('Session is already opened')

    if ip not in opened_sessions:
        opened_sessions.append(ip)
    cmdUse(ip)

    print(sendToService({ 'ip' : ip }, 'api/login'))


def cmdForceLogout(ip: str):
    global current_session
    if ip == current_session:
        current_session = ''

    print(sendToService({ 'ip' : ip }, 'api/logout'))


def cmdLogout(ip: str):
    global current_session

    if ip == 'cur':
        ip = current_session

    if ip == 'all':
        current_session = ''
        for s in opened_sessions:
            cmdForceLogout(s)
        opened_sessions.clear()
    else:
        if ip in opened_sessions:
            cmdForceLogout(ip)
            opened_sessions.remove(ip)
        else:
            print(f'Session "{ip}" is not opened')


def cmdBackground():
    global current_session
    current_session = ''


def cmdDrop(file_path: str):
    try:
        if not isSessionUsed():
            return

        if not path.exists(file_path):
            print(f'Path "{file_path}" doesn\'t exist')
            return

        with open(file_path, 'rb') as f:
            print(sendToService({   'host' : current_session }, 'api/upload',
                                {'file' : f }))
    except Exception as exc:
        print(exc)


def cmdExit():
    print('Exiting...')
    cmdLogout('all')


printHello()
try:
    while True:
        print((current_session + ' >').strip(), end=' ')

        input_raw = input()
        input_split = input_raw.split()
        cmd = input_split[0].lower()

# Some default commands
        if cmd == 'help':
            cmdHelp()
            continue

        if cmd == 'show':
            if len(input_split) != 2:
                print('show <what>')
            else:
                cmdShow(input_split[1])
            continue

        if cmd == 'forceuse':
            if len(input_split) != 2:
                print('forceuse <ip>')
            else:
                cmdForceUse(input_split[1])
            continue

        if cmd == 'use':
            if len(input_split) != 2:
                print('use <ip>')
            else:
                cmdUse(input_split[1])
            continue

        if cmd == 'login':
            if len(input_split) != 2:
                print('login <ip>')
            else:
                cmdLogin(input_split[1])
            continue

        if cmd == 'logout':
            if len(input_split) != 2:
                print('logout <ip/all>')
            else:
                cmdLogout(input_split[1])
            continue

        if cmd == 'forcelogout':
            if len(input_split) != 2:
                print('logut <ip/all>')
            else:
                cmdForceLogout(input_split[1])
            continue


        if cmd in ('exit', 'q', 'quit'):
            raise KeyboardInterrupt

# In session commands
        if cmd in ('bg', 'background'):
            cmdBackground()
            continue

        if cmd == 'drop':
            if len(input_split) < 2:
                print('drop <file_path>')
            else:
                cmdDrop(' '.join(input_split[1::]))
            continue

        print('Unknown command')
except KeyboardInterrupt:
    cmdExit()