import math

"""
***IMPORTANT: CHANGES FOR RESUBMISSION***
1. Modified the call method for user-defined functions to evaluate each arg before evaluating.
   This means that functions nested within functions now work properly rather than sometimes
   printing the string literal that is passed in. (Fixes the last piece of feedback)
   
   Example:
    (
    (DEFINE foo (x) (+ x x))
    (PRINT (foo 3))
    (DEFINE bar (x) (PRINT (foo x)))
    (bar 5)
    (DEFINE lat (x) (bar x))
    (lat 5)
    )
    
    NOW PRINTS : 
    6
    10
    10
    
    INSTEAD OF:
    6
    xx
    xx

2. Modified the PRINT method to replace python-like [ ] brackets with LISP-friendly ( ) parentheses.
   This was done by using Python's native str.replace() function on the item that is to be printed. 
   We replaced [ with (, ] with ), and , with '' to achieve the LISPiest string possible. 
   (Fixes the second to last piece of feedback)
   
   Example:
   (
   (SET A 10)
   (SET B 20)
   (SET C (CONS A B))
   (PRINT C)
   )
   
   NOW PRINTS:
   ((10) (20))
   
   INSTEAD OF:
   [[10], [20]]

3. Modified CONS implementation to remove extra [ ] (now ( )) around result.
   This was done by changing the cons from:
   lambda x, y: [[x], [y]]
   
   to:
   return [eval(args[0]), eval(args[1])]
   
   I believe this is a much cleaner implementation, and actually evaluates the args,
   and reduces the use of redundant braces in output. 

4. Added BEGIN to each test result. This fixes the first piece of feedback and results
   in a "lispier" solution to the problem of the entire file being enclosed in ( ). 
   
***END CHANGES***

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
    DEFINE              CALL
    CITATIONS:
    https://norvig.com/lispy.html 
    ( Used as the basis for parsing and environment ideas. The vast majority of operations were designed and written by EB and SP. )
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
    ADD function for environment
    sums across the list
'''
def addExp(a, b) -> Number:
    return a + b

'''
    SUB function for environment
    subtracts the 2nd arg from the 1st arg
'''    
def subExp(a, b) -> Number:
    return (a - b)

'''
    MULTIPLICATION function for environment
    multiplies across the list
'''
def multExp(a, b) -> Number:
    return a * b

'''
    DIVISION function for environment
    divides 1st element by 2nd element
'''
def divExp(a, b) -> Number:
    return (a / b)

'''
    EQUAL function for environment
    uses basic == in python so works for numbers, characters, lists, etc. 
'''
def equalExp(a, b) -> bool:
    if (a == b):
        return T_Output
    else:
        return F_Output
    
'''
    GREATER THAN function for environment
    if the 1st element is > 2nd element returns true 
'''
def gtExp(a, b) -> bool:
    if (a > b):
        return T_Output
    else:
        return F_Output

'''
    LESS THAN function for environment
    if the 1st element is < 2nd element returns true
'''
def ltExp(a, b) -> bool:
    if (a < b):
        return T_Output
    else:
        return F_Output

'''
NULL check function for environment
if x is null return true else return false
'''
def nullExp(x):
    if not x:
        return T_Output

    return F_Output

'''
Creating this dictionary allows for simple calls to functions within eval()
Also allows for recursive calls within eval()
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
        'NULL?': nullExp,
        'T': T_Output, # returns true regardless of anything
        'PRINT': lambda x: print(str(x).replace('[', '(').replace(']', ')').replace(',', '')) # prints the evaluation of expression x
    })
    return env

# Set a variable to call environment
use_env = environment()

'''
Create a new dictionary that contains the functions that a user may define.
This dictionary will be populated with functions and parameter requirements as DEFINE is called. 
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
This function provides the main functionality for our lisp interpreter program.
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
    elif op == 'CONS': # Creates and returns a cons cell with expr as car and expr as cdr: ie: (exp1 . exp2)
        return [eval(args[0]), eval(args[1])]
    elif op == 'SET': # Associate the symbol name with the value of the expression
        (symbol, exp) = args
        env[symbol] = eval(exp, env) # Add the new symbol to our environment for future access
    elif op == 'LIST?': # Returns T if the expression is not an atom
        if isinstance(eval(*args), Number) or isinstance(eval(*args), Symbol): # If argument to check evaluates to number of symbol, return false
            return F_Output
        return T_Output
    elif op == "NUMBER?": # Returns T if the expression is a number
        if isinstance(eval(*args), Number): # Call eval on the arguments passed in, check if it is a number.
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
        #print(name, ops)
        user_functions[name] = {} # Create dictionary for new function
        user_functions[name]['Params'] = params # Set parameters field
        user_functions[name]['Ops'] = ops # Set operation field
    elif op == "T":
        return T_Output
    elif op in user_functions:
        #print('op: ', op)
        expression = user_functions[op]['Ops'] # Grab expression from existing definition dict
        for i in range(len(args)): # Iterate through each arg passed in
            cur = user_functions[op]['Params'][i] # Update the environment for each param
            use_env[cur] = eval(args[i])
        return eval(expression) # Evaluate the expression with updated variables

        #env[args[1]] = # set
    else: # Procedure call
        exec = eval(op) # Grab the function to be used to operate on
        vals = [eval(arg) for arg in args] # Evaluate every argument
        return exec(*vals) # Evaluate each argument's result in exec

'''
Function for evaluating all the expressions found in the lisp program
makes sure to separate all test cases for easy reading.
'''
def evaluateAll(expressions):
    for exp in expressions:
        eval(exp)
    print('\n')
    
### TEST CASES BELOW ###
    
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
(10 20)
10
(20)
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
(10 20)
10
(20)
t
()
t
t
t
2.0
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
(10 20)
'''

print("TEST CASE 8")
program8 = readFile("TestLisp8.txt")
list8 = parser(program8)
evaluateAll(list8)
'''
Should print:
11
'''

print("TEST CASE 9")
program9 = readFile("TestLisp9.txt")
list9 = parser(program9)
evaluateAll(list9)
'''
Should print:
6
10
10
'''
