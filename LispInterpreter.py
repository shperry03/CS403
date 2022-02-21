from ast import Num
import math
import operator as op
from selectors import EpollSelector
import numpy



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

def environment() -> Env:
    env = Env()
    env.update(vars(math))
    env.update({
        '+': addExp,
        '-': subExp,
        '=': equalExp,
        '>': gtExp,
        '<': ltExp,
        '*': multExp,
        '/': divExp,
        'BEGIN': lambda *x: x[-1], # sets the last element of the list to the beginning
        'CAR': lambda x: x[0], # returns the first value in the list
        'CDR': lambda x: x[1:], # returns the remaining elements in the list
        'CONS': lambda x,y: [x] + y, # rerturns a list pair of x and y (x . y) where x and y are both lists
        'NUMBER?': lambda x: isinstance(x, Number), # checks if x is a Number/(int,float) 
        'SYMBOL?': lambda x: isinstance(x,Symbol), # checks if x is a Symbol/str
        'LIST?': lambda x: isinstance(x,list), # checks if x is a list
        'NULL?': lambda x: x == [], # returns the comparison of x == [] a null list
        'T': True, # returns true regardless of anything
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
def eval(exp: list, env = use_env) -> Exp:
    if isinstance(exp, Symbol): # If item is a symbol, we want to return the corresponding operation
        return env[exp]
    elif isinstance(exp, Number): # If item is a number, return it
        return exp
    elif exp[0].upper() == 'SET': # If the first item is SET
        (_, symbol, exp) = exp # Set three variables based on the required 3 items for set
        env[symbol] = eval(exp, env) # Add the new symbol to our dictionary of characters
    elif exp[0].upper() == 'DEFINE':
        print()
    else: # Item is not an atom or known definition
        calc = eval(exp[0].upper(), env) # Get the first character (we know we will evaluate on this)
        args = [eval(arg, env) for arg in exp[1:]] # Call eval recursively on every other item
        return calc(*args) # Call the correct evaluation based on symbol one and the results of recursive calls
    
'''
    ADD function for DICT
    sums across the list
'''
def addExp(x: list) -> Number:
    return sum(x)

'''
    SUB function for DICT
    subtracts the 2nd arg from the 1st arg
'''    
def subExp(x: list) -> Number:
    return (x[0] - x[1])

'''
    MULTIPLICATION function for DICT
    multiplies across the list
'''
def multExp(x: list) -> Number:
    return numpy.prod(x)

'''
    DIVISION function for dict
    divides 1st element by 2nd element
'''
def divExp(x: list) -> Number:
    return (x[0] / x[1])

'''
    EQUAL function for dict
    uses basic == in python so works for numbers, characters, lists, etc. 
'''
def equalExp(x: list) -> bool:
    if (x[0] == x[1]):
        return True
    else:
        return False
    
'''
    GREATER THAN function for DICT
    if the 1st element is > 2nd element returns true 
'''
def gtExp(x: list) -> bool:
    if (x[0] > x[1]):
        return True
    else:
        return False

'''
    LESS THAN function for dict
    if the 1st element is < 2nd element returns true
'''
def ltExp(x: list) -> bool:
    if (x[0] < x[1]):
        return True
    else:
        return False


program = readFile("TestLisp.txt")

list1 = parser(program)
print(list1)
print(variables)
print(eval(list1))
