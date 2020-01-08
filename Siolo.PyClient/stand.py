from brs_exc import BadResponseStateException

try:
  raise BadResponseStateException('hello')
except BadResponseStateException as exc:
  print(exc)