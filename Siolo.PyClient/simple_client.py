import requests
import base64
import json
from os import path
from brs_exc import BadResponseStateException

SERVICE_IP = 'localhost'
SERVICE_PORT = 52925

current_session = ''
opened_sessions = []

def sendToService(data: dict, cmd_addr: str, files: dict = None):
    url = f'http://{SERVICE_IP}:{SERVICE_PORT}/{cmd_addr}/'
    if files is None:
        r = requests.post(url, json=data)
    else:
        r = requests.post(url, data=data, files=files)

    parsed_obj = json.loads(r.text)

    if parsed_obj['status'] == False:
        raise BadResponseStateException(parsed_obj['message'])

    return parsed_obj['message']



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
    print('show <what>          -   show smth (sessions, incs, active_hosts)')
    print('forceuse <ip>        -   use not opened session in current client')
    print('use <ip>             -   use opened session in current client')
    print('login <ip>           -   login ip to system and open session (it will register ip for first time)')
    print('forcelogout <ip>     -   force logut ip from system')
    print('logout <ip/all/cur>  -   logut ip from system')
    print('report <what> <args> -   print details about smth (inc inc_id, file hash)')
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

    if what == 'incs':
        hits = json.loads(sendToService({}, 'api/find_incs'))

        for i, data in [(x['Id'], x['Data']) for x in hits]:
            print(f"{i} {data['LogDate']} {data['LogTime']}\t| File hash: {data['Md5']} | IP: {data['Ip']}")

        return

    if what == 'active_hosts':
        hosts = json.loads(sendToService({}, 'api/get_active_hosts'))
        for i, s in zip(range(len(hosts)), hosts):
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


def cmdReport(args: list):
    if args[0] == 'file':
        short_report = json.loads(sendToService({'hash': args[1]}, 'api/get_short_report', {}))
        print('*' * 22 + ' File report ' + '*' * 22)
        print('MD5:\t\t\t' + short_report['md5'])
        print('File size:\t\t' + str(short_report['file_size']) + ' bytes')
        print('Full class:\t\t' + short_report['full_class'])
        print('Detection engine:\t' + short_report['detection_engine'])
        print('Report created:\t\t' + short_report['report_date'] + ' ' + short_report['report_time'])
        print('*' * 57)
        return

    if args[0] == 'inc':
        inc = json.loads(sendToService({'id': args[1]}, 'api/find_inc', {}))
        if len(inc) == 0:
            print(f'Inc "{args[1]}" not found')
            return

        inc = inc[0]

        head = '=' * 25 + ' Incident ' + args[1] + ' ' + '=' * 25
        print(head)

        # TODO: remove this fucking ternary expressions after debug logstash + this function and its API (but not for PossibleRoutes)
        print('IP:\t\t\t' + (inc['ip'] if inc['ip'] is not None else 'Unknown'))
        print('Incident register date:\t' + inc['logdate'])
        print('Incident register time:\t' + inc['logtime'])
        print('Triggered by policy:\t' + (inc['RestrictingPolicy'] if inc['RestrictingPolicy'] is not None else 'Unknown'))

        cmdReport(['file', '3af1008ba9f6dddaf99907d9458ee775'])

        print('First occurrence IP: ' + (inc['PossibleRoutes'][0][0] if len(inc['PossibleRoutes']) > 0 else 'Unknown'))

        if len(inc['PossibleRoutes']) > 0:
            print('Possible routes:')
            for i, route in zip(range(len(inc['PossibleRoutes'])), inc['PossibleRoutes']):
                print(str(i) + '.\t' + ' ->\n\t'.join(route))

        print('=' * len(head))
        
        return


    print(f'Cant show report for "{args[0]}"')

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

        if cmd == 'report':
            if len(input_split) != 3:
                print('report <what> <args> (inc inc_id, file hash)')
            else:
                cmdReport(input_split[1::])
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