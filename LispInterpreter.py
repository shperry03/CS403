
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

'''
variables is a global dictionary that will be used to store values of variables found in the Lisp file
'''
variables = {}

Symbol = str
Number = (int, float)
Atom = (Symbol, Number)
Exp = (Atom, list)
Env = dict
F_Output = '()'
T_Output = 't'


'''
    ADD function for DICT
    sums across the list
'''
def addExp(a, b) -> Number:
    return a + b

'''
    SUB function for DICT
    subtracts the 2nd arg from the 1st arg
'''    
def subExp(a, b) -> Number:
    return (a - b)

'''
    MULTIPLICATION function for DICT
    multiplies across the list
'''
def multExp(a, b) -> Number:
    return a * b

'''
    DIVISION function for dict
    divides 1st element by 2nd element
'''
def divExp(a, b) -> Number:
    return (a / b)

'''
    EQUAL function for dict
    uses basic == in python so works for numbers, characters, lists, etc. 
'''
def equalExp(a, b) -> bool:
    if (a == b):
        return T_Output
    else:
        return F_Output
    
'''
    GREATER THAN function for DICT
    if the 1st element is > 2nd element returns true 
'''
def gtExp(a, b) -> bool:
    if (a > b):
        return T_Output
    else:
        return F_Output

'''
    LESS THAN function for dict
    if the 1st element is < 2nd element returns true
'''
def ltExp(a, b) -> bool:
    if (a < b):
        return T_Output
    else:
        return F_Output

def environment() -> Env:
    env = Env()
    env.update({
        '+': addExp,
        '-': subExp,
        '=': equalExp,
        '>': gtExp,
        '<': ltExp,
        '*': multExp,
        '/': divExp,
        'BEGIN': lambda *x: x[-1], # sets the last element of the list to the beginning
        'CONS': lambda x,y: [[x], [y]], # rerturns a list pair of x and y (x . y) where x and y are both lists
        'NUMBER?': lambda x: isinstance(x, Number), # checks if x is a Number/(int,float) 
        'SYMBOL?': lambda x: isinstance(x,Symbol), # checks if x is a Symbol/str
        'LIST?': lambda x: isinstance(x,list), # checks if x is a list
        'NULL?': lambda x: x == [], # returns the comparison of x == [] a null list
        'T': T_Output, # returns true regardless of anything
        'PRINT': lambda x: print(x) # prints the evaluation of expression x
    })
    return env

use_env = environment()
    
'''
Open the Lisp file for reading and return it in string form
'''
def readFile(LispFile: str) -> str:
    fileObject = open(LispFile, "r") # Open the Lisp file (written by us) for reading
    program = fileObject.read() # Conver the file into one large string
    return program

'''
Calls readTokens on the string version of the Lisp file and returns the list of commands
'''
def parser(program: str) -> list:
    return readTokens(tokenize(program))

'''
Takes in a piece of text (a Lisp file in string form) and creates tokens, separating on ()
'''
def tokenize(chars: str) -> list:
    return chars.replace('(',' ( ').replace(')' , ' ) ').split()

'''
Takes the list of tokens and turns it into a list of lists by nesting statements in () properly
'''
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
                return Symbol(t)

    
'''
Evaluates LISP statements recursively.
This function provides the main functionality for our lisp program.
'''
def eval(exp, env = use_env) -> Exp:
    "Evaluate an expression in an environment."
    if isinstance(exp, Symbol):    # variable reference
        return env[exp]
    elif not isinstance(exp, list):# constant 
        return exp 
    op, *args = exp     
    if op == 'IF':             # conditional
        (test, conseq, alt) = args
        exp = (conseq if eval(test, env) == 't' else alt)
        return eval(exp, env)
    elif op == 'WHILE':
        (exp1, expBody) = args
        while (eval(exp1) == 't'):
            eval(expBody)
    elif op == 'CAR':
        return args[0]
    elif op == 'CDR':
        return args[1:]
    elif op == 'SET':         # definition
        (symbol, exp) = args
        env[symbol] = eval(exp, env)
    elif op == 'LIST?':
        return isinstance(args, list)
    else:                        # procedure call
        proc = eval(op, env)
        vals = [eval(arg, env) for arg in args]
        if proc is None:
            exit()
        return proc(*vals)


program = readFile("TestLisp.txt")

list1 = parser(program)
print(list1)
print(variables)
print(eval(list1))
