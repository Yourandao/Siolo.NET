import requests
import base64
import json
import sys
from os import path
from brs_exc import BadResponseStateException

SERVICE_IP = 'localhost'
SERVICE_PORT = 52925

current_session = ''
opened_sessions = []

script_name = sys.argv[0][sys.argv[0].rfind('\\') + 1::]

file_commands = []

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
    print(f"""\
{script_name} commands_file.txt (optional)

Available commands:
help                 -   print help information
show <what>          -   show smth (sessions, incs, active_hosts)
forceuse <ip>        -   use not opened session in current client
use <ip>             -   use opened session in current client
login_sn <subnet_ip> -   login subnet to system (can be used for adding persisting hosts)
login <ip>           -   login ip to system and open session (it will register ip for first time)
forcelogout <ip>     -   force logut ip from system
logout <ip/all/cur>  -   logut ip from system
report <what> <args> -   print details about smth (inc inc_id, file hash)
    
In session commands:
background           -   background current session
drop <file_path>     -   trigger drop file event
    
Admin functions:
admin <what> <args>
<what>:
    wildcard <wildcard> <info>  -   create new policy
    attach  <ip> <wildcard>     -   attach wildcard to ip
    link <ip_1> <ip_2>          -   make specific relation between hosts (for subnets)

    
exit                 -   disconnect all sessions and leave script\
""")


def cmdShow(what: str):
    if what == 'sessions':
        for i, s in zip(range(len(opened_sessions)), opened_sessions):
            print(f'{i}: {s}')
        return

    if what == 'incs':
        hits = json.loads(sendToService({}, 'api/find_incs'))

        for i, data in [(x['Id'], x['Data']) for x in hits]:
            print(f"{i} {data['logdate']} {data['logtime']}\t| File hash: {data['md5']} | IP: {data['ip']}")

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


def cmdLoginSn(ip: str):
    print(sendToService({ 'ip' : ip }, 'api/login'))


def cmdLogin(ip: str):
    if ip in opened_sessions:
        print('Session is already opened')
        return

    cmdLoginSn(ip)
    opened_sessions.append(ip)
    cmdUse(ip)


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

        print()
        cmdReport(['file', inc['md5']])
        print()

        print('First occurrence IP: ' + (inc['PossibleRoutes'][0][0] if len(inc['PossibleRoutes']) > 0 else 'Unknown'))

        if len(inc['PossibleRoutes']) > 0:
            print('Possible routes:')
            for i, route in zip(range(len(inc['PossibleRoutes'])), inc['PossibleRoutes']):
                print(str(i) + '.\t' + ' ->\n\t'.join(route))

        print('=' * len(head))
        
        return


    print(f'Cant show report for "{args[0]}"')


def cmdAdmin(cmd:str, args: list):
    if cmd == 'wildcard':
        if len(args) != 2:
            print('wildcard <wildcard> <info>  -   create new policy')
        else:
            print(sendToService({'Wildcart': args[0], 'Info': args[1]}, 'api/wildcart'))
        return

    if cmd == 'attach':
        if len(args) != 2:
            print('attach  <ip> <wildcard>     -   attach wildcard to ip')
        else:
            print(sendToService({'Ip': args[0], 'Wildcart': args[1]}, 'api/attach'))
        return

    if cmd == 'link':
        if len(args) != 2:
            print('link <ip_1> <ip_2>          -   make specific relation between hosts (for subnets)')
        else:
            print(sendToService({'First': args[0], 'Second': args[1]}, 'api/link'))
        return

    print(f'Unknown command "{cmd}"')

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


if len(sys.argv) > 2:
    print(f'{script_name} commands_file.txt (optional)')
    exit(0)


def processCommand(command: str):
    input_raw = command
    input_split = input_raw.split()
    cmd = input_split[0].lower()

# Some default commands
    if cmd == 'help':
        cmdHelp()
        return

    if cmd == 'show':
        if len(input_split) != 2:
            print('show <what>')
        else:
            cmdShow(input_split[1])
        return

    if cmd == 'forceuse':
        if len(input_split) != 2:
            print('forceuse <ip>')
        else:
            cmdForceUse(input_split[1])
        return

    if cmd == 'use':
        if len(input_split) != 2:
            print('use <ip>')
        else:
            cmdUse(input_split[1])
        return

    if cmd == 'login_sn':
        if len(input_split) != 2:
            print('login_sn <ip>')
        else:
            cmdLoginSn(input_split[1])
        return

    if cmd == 'login':
        if len(input_split) != 2:
            print('login <ip>')
        else:
            cmdLogin(input_split[1])
        return

    if cmd == 'logout':
        if len(input_split) != 2:
            print('logout <ip/all>')
        else:
            cmdLogout(input_split[1])
        return

    if cmd == 'forcelogout':
        if len(input_split) != 2:
            print('logut <ip/all>')
        else:
            cmdForceLogout(input_split[1])
        return

    if cmd == 'report':
        if len(input_split) != 3:
            print('report <what> <args> (inc inc_id, file hash)')
        else:
            cmdReport(input_split[1::])
        return

    if cmd == 'admin':
        if len(input_split) < 3:
            print('admin <what> <args>')
        else:
            cmdAdmin(input_split[1], input_split[2::])
        return


    if cmd in ('exit', 'q', 'quit'):
        raise KeyboardInterrupt

# In session commands
    if cmd in ('bg', 'background'):
        cmdBackground()
        return

    if cmd == 'drop':
        if len(input_split) < 2:
            print('drop <file_path>')
        else:
            cmdDrop(' '.join(input_split[1::]))
        return

    print('Unknown command')


# Load commands from file if 
if len(sys.argv) == 2:
    try:
        with open(sys.argv[1], 'r') as f:
            file_commands = [x.strip() for x in f.readlines()]
    except:
        pass


printHello()
try:
    for command in file_commands:
        if command == '':
            continue
        
        print((current_session + ' > ' + command).strip())
        processCommand(command)
        
    while True:
        print((current_session + ' >').strip(), end=' ')
        processCommand(input())
except KeyboardInterrupt:
    cmdExit()