import math

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

    CITATIONS:
    https://norvig.com/lispy.html ( used as the basis for parsing and environment ideas )

"""

'''
Setting up the variables and terms in list
translates the python variables to lisp for easy use
and sets basics of language
'''
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

'''
NULL check function for dictionary
if x is null return true else return false
'''
def nullExp(x):
    if not x:
        return T_Output

    return F_Output

'''
Creating this dictionary allows for simple calls to functions within eval()
Allows for recursive calls within eval()
'''
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
        'CONS': lambda x,y: [[x], [y]], # rerturns a list pair of x and y (x . y) where x and y are both lists
        'NULL?': nullExp,
        'T': T_Output, # returns true regardless of anything
        'PRINT': lambda x: print(x) # prints the evaluation of expression x
    })
    return env

# Set a variable to call environment
use_env = environment()

'''
Create a new dictionary that contains the functions that a user may define
'''
def user_functions() -> Env:
    env = Env()
    return env

user_functions = user_functions()

    
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
    if isinstance(exp, Symbol): # Check if exp is an instance of Symbol and return its corresponding operation
        return env[exp]
    elif isinstance(exp, Number): # Check if an exp is an instance of a number and return it if so
        return exp
    elif not isinstance(exp, list): # If exp is any other non-list item, return it
        return exp
    op, *args = exp # Grab the operator and the rest of the command
    if op == 'IF': # IF expression
        (exp1, expT, expF) = args # Grab the three arguments that should have been passed in
        exp = (expT if eval(exp1, env) == 't' else expF) # If evaluating the test expression yields true, return the first output, otherwise the second
        return eval(exp, env)
    elif op == 'WHILE': # WHILE expresion
        (exp1, expBody) = args # Grab the test expression and the expression to be executed
        while (eval(exp1) == 't'): # While the first expression holds true, evaluate the second expression 
            eval(expBody) # (Second expression must eventually cause first expression to yield T)
    elif op == 'CAR': # Return the first argument of args
        return eval(args[0])[0]
    elif op == 'CDR': # Return all but the first arguments of args
        return eval(args[0])[1:]
    elif op == 'SET': # Associate the symbol name with the value of the expression
        (symbol, exp) = args
        env[symbol] = eval(exp, env) # Add the new symbol to our environment for future access
    elif op == 'LIST?': # Returns T if the expression is not an atom
        if isinstance(eval(*args), Number) or isinstance(eval(*args), Symbol): # If argument to check evaluates to number of symbol, return false
            return F_Output
        return T_Output
    elif op == "NUMBER?": # Returns T if the expression is a number
        if isinstance(eval(*args), Number):
            return T_Output
        return F_Output
    elif op == "SYMBOL?": # Returns T if exp is a name, () otherwise
        if isinstance(eval(*args), Symbol): # Check if exp evaluates to a symbol
            return T_Output
        return F_Output
    elif op == "DEFINE": # Defines a function. When called the expression will be evaluated with the actual parameters
        # define name (arg1 ... argN) expr)
        name = args[0] # grab name
        params = args[1] # grab params
        ops = args[2] # grab arguments
        user_functions[name] = {} # Create dictionary for new function
        user_functions[name]['Params'] = params # Set parameters field
        user_functions[name]['Ops'] = ops # Set operation field
    elif op == "T":
        return T_Output
    elif op in user_functions:
        expression = user_functions[op]['Ops'] # Grab expression from existing definition dict
        for i in range(len(args)): # Iterate through each arg passed in
            cur = user_functions[op]['Params'][i] # Update the environment for each param
            use_env[cur] = args[i]
        return eval(expression) # Evaluate the expression with updated variables

        #env[args[1]] = # set
    else: # Procedure call
        exec = eval(op) # Grab the function to be used to operate on
        vals = [eval(arg) for arg in args] # Evaluate every argument
        return exec(*vals) # Evaluate each argument's result in exec

'''
function for evaluating all the expressions found in the lisp program
makes sure to separate all test cases for easy reading
'''
def evaluateAll(expressions):
    for exp in expressions:
        eval(exp)
    print('\n')
        
    
print("TEST CASE 1")
program1 = readFile("TestLisp1.txt")
list1 = parser(program1)
#print(list1)
evaluateAll(list1) 
'''
Should print:
-10
0.1
()
t
t
()
'''

print("TEST CASE 2")
program2 = readFile("TestLisp2.txt")
list2 = parser(program2)
#print(list2)
evaluateAll(list2)
'''
Should print:
[10], [20]
[10]
[20]
'''

print("TEST CASE 3")
program3 = readFile("TestLisp3.txt")
list3 = parser(program3)
#print(list3)
evaluateAll(list3)
'''
Should print:
(nothing)
'''

print("TEST CASE 4")
program4 = readFile("TestLisp4.txt")
list4 = parser(program4)
evaluateAll(list4)
'''
Should print:
22
'''

print("TEST CASE 5")
program5 = readFile("TestLisp5.txt")
list5 = parser(program5)
evaluateAll(list5)
'''
Should print:
0
'''

print("TEST CASE 6")
program6 = readFile("TestLisp6.txt")
list6 = parser(program6)
evaluateAll(list6)
'''
Should print:
[10]
[[20]]
t
()
t
t
2
'''

print("TEST CASE 7")
program7 = readFile("TestLisp7.txt")
list7 = parser(program7)
evaluateAll(list7)
'''
Should print:
t
t
()
()
[[10], [20]]
'''

print("TEST CASE 8")
program8 = readFile("TestLisp8.txt")
list8 = parser(program8)
evaluateAll(list8)
'''
Should print:
11
'''
