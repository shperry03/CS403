
"""
Sam Perry and Erik Buinevicius

Our project is a Lisp Interpreter, based on Kamin's Lisp in Pascal, written in python.

It will read text files in Lisp Style and execute certain commands:

    WHILE               CONS
    SET                 CAR
    BEGIN               CDR
    +                   NUMBER?
    -                   SYMBOL?
    *                   LIST?
    /                   NULL?
    =                   PRINT
    <                   T
    >                   IF

"""

'''
Test Changes Erik
'''
'''
Test Changes Sam
'''

variables = {}

def readFile(LispFile: str) -> str:
    fileObject = open(LispFile, "r")
    program = fileObject.read()
    return program

def parser(program: str) -> list:
    return readTokens(tokenize(program))

def tokenize(chars: str) -> list:
    return chars.replace('(',' ( ').replace(')' , ' ) ').split()

def readTokens(tokens: list):
    t = tokens.pop(0)
    if t == '(':
        newList = []
        while tokens[0] != ')':
            # can check for number here instead of adding 
            # number as a string
            newList.append(readTokens(tokens))
        tokens.pop(0)
        return newList
    else:
        # try to return int
        try: return int(t)
        except ValueError:
            # if fails, try to return float
            try: return float(t)
            except ValueError:
                # if all fails, just return it as a string
                return str(t)


'''
Set statement action
'''
def setStatement(token: list):
    # if var exists, update it, else add it to dict
    variables[token[1]] = token[2]


def get_inner(lst):
    print()

program = readFile("TestLisp.txt")

list1 = parser(program)
print(list1)
print(variables)