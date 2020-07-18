# Setup

* Install CLI `dotnet` 
* Restore Project `dotnet restore`


# Build the language as the C# library

`dotnet build`


# Run the tests

`dotnet test`

also to run the test with `watch`

`dotnet watch --project HerbertLang.Tests tes`


# Notes 

```
-> Solution  
   - HerbertLang (There is a reference set to TEST dotnet add ... reference)
   - HerbertLang.Tests 
```

# Flow 

```
LEXER -> tokens
PARSER -> ast
INTERPRETER -> (code/string | code/steps)
SOLVER (code, world) -> solution
```

# Idea 

```python

   # simple 
   sss

   # function no args
   f:sss
   f

   f():sss
   f()

   # function step args
   f(A):A
   f(sss)

   f(B):aBa
   f(sss)

   # function deep args
   f:f=@
   f=0

```