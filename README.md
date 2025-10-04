# ZeroCode2
Code generator based on YAML-like definition of your project

# Model specification

The Model can be split over several files, that are combined into one model (through the "&" directive)
The model consists of a series of sections. Each section consists of a number of objects: an entity that has a name and a number of properties. Each property is either a single value (string , number or Boolean), or an object itself.

A section starts with a @ followed by its name (one word).

A section can also start with # and a name, in which case it contains a series if key-value pairs, similar to normal section's objects.

Objects are delimited by "{" "}" pairs, and properties are separated by comma's.
A property's name and its value are separated by ":".

The model supports inheritance, at each level and for each entity: an entity may inherit from another entity. A property may inherit from another entity. This is indicate by "<-" after the name of the entity or property. An inherited property/entity may add, remove or change properties of the inherited entity (locally). Adding is done by specifying the new property, preceded by a "+"; removing a property is preceded by a "-". In case a property is added, its value must be specified. In case a property is removed, only its name (preceded by a minus) is specified. Changing a property's definition is simply done by specifying its name followed by its new value.

An object that has inherited properties can change the order in which its properties are presented, by specifying this order at the end of the object's properties, preceded by a "/": / prop1, prop2, prop3. non-specified properties are added to the end of this list automatically. See below for a full description.

The format resembles JSON, with the exception that the array notation ("[", "]") is not supported. In addition, the notion of sections was added,  and the support of inheritance. Sub-parts of the model can be stored in a separate file, that can be included through the "&" directive, followed by the path + filename of the file to be included.

Standard C-like comments (// and /* */) are supported.

# Template specification

A template is a combination of literal text and commands that act upon the provided model. The  tool outputs the result of this combination in a series of files. Commands can inject values from the model in the text that is output. Commands can, based on values from the model, include or exclude certain parts of the model (if-then-else). Commands can repeatedly output certain sections of the template (including elements from the model), looping over entities from the model.

The template specification can contain any form of text. Commands all start with a "%", followed by one of the following keywords, a colon ":" and the command's parameter:

Keyword	- Explanation	- Command parameter
# FileCreate	
Creates a file with the name as specified after this keyword. If the file already exists, it is not recreated and no text is added to the file. This command sits on a line of its own.	Path and filename to send the output to. Supports expressions.
  
# FileOverwrite	
Creates a file as FileCreate, but regardless if it already exists or not. It is always recreated. This comments sits on a line of its own.	Same as FileCreate
	
# Loop	
Repeatedly loops over the elements of the model. The element to loop over is specified after the Loop keyword and it must conform to the "path" in the model (see later).	Path in the model to loop over. Supports relative path to the nearest enclosing loop. In case the path refers to another loop, it shall name that loop path explicitly.

A Loop statement can take up a single line of be part of a piece of text. In the latter case, the Loop statement must be followed by a space. This space is considered part of the Loop statement it is not emitted to the result.

## Existance of properties to loop over
A loop path can optionally end with a question mark ("?"), making the loop optional. If the element to loop over (the path) does not exist in the model, the loop will thus be skipped entirely. Omitting the ? qualifier in the path will generate a runtime error if the path does not exist. In the template, this construct can also be achieved by enclosing the loop with an If statement that tests for the path(e.g. %If:path? %Loop:path ... %/Loop:path %EndIf). However using the optional qualifier in the loop path itself is more succinct and elegant.

## Property order
Ordering of the elements in the loop follows the way they are present in the model (natural order).
In case properties are inherited, the inherited ones come first, then the added properties. Properties that are overridden, do not change their ordering.
		
NOTE: there is currently no way to change the ordering through a loop statement (order by not supported).

## Loop filtering (not supported)
When looping over a property, it is sometimes needed to only consider a subset of the properies' elements. This is easily achieved by adding an %If inside the loop. 

	%Loop:SomeProperty
	%If:SomeProperty.SomeInternalPropery=someValue
		... emit conditionally
	%EndIf
	%/Loop:SomeProperty

Often however such constructs are overly verbose, in particular when the condition to test for is only to restrict the elements in the loop to a subset.

A proposed solution is to enhance the Loop syntax by adding an optional filtering clause:

	%Loop:Property|Filter
	%/Loop:Property

The "|" was chosen for notational brevity. It reminds of the pipe symbol in shell languages, and has a similar function: the loop elements "pass through" the filter and only if the filter clause is true, the element is passed into the loop's body. 

Using a loop filter is semantically fully equivalent to the Loop/If combination mentioned above. The Filter element's syntax is the same as the condition in an If clause. 
As a consequence, a filter condition can refer to any valid condition in the frame of the model. It is not restricted to the loop path.  
Note that alternatives are not supported (there is no notion of Else in the filter, and it would be useless to have this in a loop).

# If	
Tests for a condition, which is a property from the model, which is tested against a fixed value. Negative testing is done by prefixing the test by a "!".  Existence of a property be tested by suffixing it by "?".

It is followed by a colon and a path to a model expression, followed by "=" or "!=" and a literal value (no spaces allowed).

An If statement can take up a single line of be part of a piece of text. In the latter case, the If statement must be followed by a space. This space is considered part of the If statement it is not emitted to the result.	May be preceded by "!" to negate the complete test.
			
A path to a model element can be tested for existence by appending it by "?".  This can be used alone to test if the property is present in the model, or in a comparison. In the latter case, the test fails if the property does not exist, and if it exists, the comparison against the compare value is performed. Preceding it with "!" negates the complete comparison.
			
If a path element is a boolean by itself, the "="+value can be omitted.
			
			Example:
			%If:SomeProperty?=somevalue
			        - If SomeProperty does not exist: false
			        - If it exists, and it is somevalue: true
			        - Otherwise: false
			%If:!SomeProperty?=somevalue
			        - If SomeProperty does not exist: true
			        - If it exists, and it is somevalue: false
			        - Otherwise: true
			%If:SomeProperty?!=somevalue
			        - If SomeProperty does not exist: false
			        - If it exists, and it is somevalue: false
			        - Otherwise: true
			%If:!SomeProperty?!=somevalue
			        - If SomeProperty does not exist: true
			        - If it exists, and it is somevalue: true
			        - Otherwise: false
# Else	
  Specifies that alternative of the "If", which is taken if the test results in "false". Else is optional. The same rules for placement (on a line of its own or not) as for If apply.
  
# EndIf	
  Ends the "If" section. It is not optional: every If must be ended with EndIf. The same rules for placement (on a line of its own or not) as for If apply.	
	
# Include	
  Includes a sub-template in the template file. The Include statement sits on a line of its own.	Path and filename to include. No support for expressions. (the include is executed during the reading of the template, not while executing it).
	
# /File	
  This statement indicates that the end of the accompanying FileCreate or FileOverwrite. Any text emitted (so all the literals and commands between the FileCreate/FileOverwrite and /File is actually written to the file at this point only. In case of a FileCreate, where the file already existed, the code generation tool will continue at the point after the /File statement (without emitting anything).	Optional text to indicate which file we are generating; this text is ignored.
		This statement sits on a line of its own.
	
# /Loop	
  End a "Loop" statement. Anything between a Loop and /Loop is emitted as many times as the loop expression has elements.	Optional text to indicate which loop we are closing. If present, it is ignored. If such text is present, the statement must be followed by a colon ":". If no text follows, the ":" does not need to be specified.
		The same rule for spacing applies as for "Loop"
	
# Logging	
A number of logging statements exist, which send log messages to the logging system of the tool. They all behave the same: any text after the log statement until the end of the line, is sent to the logging system. Consequently, this statement sits on a line of its own. The logging statements follow a severity model; the following commands are supported:
		        - Trace
		        - Info
		        - Debug
		        - Error
All text after the colon until the end of the line if logged.

Apart this, the "Log" command exists as well, but this will always send output to the console, without actually logging it.
	
	
# Expressions
	
An expressions refers to an element in the model. It has the form of "=<expression>" in the template. The expression refers to either an explicit path into the model, or to a path relative to one of the loops enclosing the expression.
Expression syntax can also be present in File commands, to make the filename to be used dependent on the model. Logging commands also support expression syntax.
	
An expression can itself contain sub-expressions. These are indicated by a "[expression]" inside the expression. Such a sub-expression is evaluated before the expression resulting from it itself is evaluated. These sub-expressions can be recursive ( sub-expression can itself contain another subexpression). 
Thus, a property of the model can itself contain a value that contains a sub-expression. This is also evaluated before the resulting text is emitted.
	
	Person : {
		ID : { Type:"int", Name:"ID of Person", Title:"ID"},
		Name : { Type:"string", Name:"Name", Title:"Name", Length:50, Nullable:false },
		Title : { Type:"string", Name:"Title", Title:"Title", Length:100, Nullable:false, SomeOtherProperty <- DataDictionary.NameField : { Name : "Another name", +Something : true } },
		InheritedField <- DataDictionary.NameField,
		CodeField : "<Input Type='[@Models.Person.Name.Type]' >"
	}
	
Evaluation the expression =<Person.CodeField>" will give "<Input Type='string' >" as result.
(note that this means that the use of "[" and "]" in the model's values is restricted).

# Defining new properties
One specific form of expression allows defining new properties during execution. They take the form of =<property=somevalue>. From that point onwards, this propery can be referred to and will have a (string) value as specified (e.g. "somevalue" in the example). The thus defined property can be set to subsequent new values if required. Using =<property=> (no value) will remove the propery from the model (i.e. it will no longer exist from that point onwards). Referring to a non-existing property will generate a runtime error.
	
# Model paths
  
A path through the model is a sequence of property names, separated by dots. This sequence, when followed from the outer/highest level element of the model to the end will yield the value of that last element. It can thus be a literal value (string, number, Boolean), or a reference to an object.
	
When using a path in an expression, it will yield the value of the property at the "end" of the path.
	
When using a path in a Loop statement, the loop will iterate through each element of the property at the end of the path. This property is therefore expected to be an object.
	
When looping through an object, the name of the current element is designated by "$".
	
Paths can be relative to the path indicated in a loop. By default, a relative path refers to the closest (innermost) loop. Reference to loops further out can be identified by using the path from the loop (being a fully qualified path, i.e. from the start of a section in the model or a relative path itself).
	
Alternatively, the construct "Loop" followed by a number (Loop0, Loop1) can be used, where the number designates the level of the loop, counting from outermost (0) to innermost.
	
When referring to a path in a loop, the expression shall use the exact path as mentioned in the loop. In a sense, the path in the loop statement becomes the loop identifier. In case loop paths are repeated in nested loops (this is not impossible), the nearest loop with the matching path is selected. If an outer loop needs to be referred to, use the Loop<number> construct.
	
	Examples:
	Given model as follows:
	#Parameters
	Pr1 : "one"
	Pr2: "two"
	
	@Section
	
	S1 : {
		SP1 : 1,
		SP2: "S1P2",
		SP3: { p1: 1 }
	},
	S2 : {
		SP1: 2,
		SP2: "S2P2",
		SP3: { p1: 1 }
	}
	
	// end model
	
	Expressions:
	=<#Parameters.Pr1> gives "one"
	
	Expressions in a loop context:
	
	%Loop:#Parameters
		=<$> // gives "one", then "two"
		=<#Parameters.$> // same
	%/Loop
	
	%Loop:@Section
		=<$> // gives S1, then S2
		%Loop:SP3
			=<Section.SP1> // gives 1 then 2
			=<Loop0.SP1> // same result
			=<p1> // gives 1 then 1
		%/Loop
		%Loop:@Section.S1 // this is a fully qualified path, not a reference
			=<$> // gives 1, S1P2, { 1 }
			=<Loop1.$> // same result
		%/Loop
		%Loop:Section.SP3 // this refers to the outermost loop (Section)
			=<$> // gives 1 (p1), then 1 (p1)
			=<@Section.$> // gives S1, then S2
			=<Section.$> // gives S1, then S2
			=<@Section.S1.SP1> // gives 1, then 1, not a reference to a loop!
			=<Section.S1.SP1> // invalid, there is no S1 element in the loop designated by Section - note that Section in this context refers to the element that is inside @Section.
		%/Loop
	%/Loop
	
# Ordering of properties in an inherited object definition
Ordering of properties is defined by the inheritance path. The default behaviour is that inherited properties are presented first, followed by any added properties. Property overriding does not change the property's order. So, explicit ordering of properties in an object that uses inheritance requires an extra semantic.
	
Objects that have no inheritance do not need this feature, because the ordering needed can be specified by the natural ordering of its properties. Hence, specifying an ordering clause on non-inherited objects will issue a warning and the ordering clause is ignored.
	
By adding the a specific order clause at the end of an object definition, the ordering of properties can be altered. This includes the inherited properties, which are normally not accessible without explicitly overriding them.
	
The properties mentioned in the ordering clause are presented in a loop expression in that order, followed by the properties that were not mentioned in the ordering clause. These remaining properies are presented in the default order, meaning that properties from an inherited element that was ordered, are presented in that order. The ordering clause thus does not filter properties from the model, it merely moves the specified properties to the beginning of the loop order.
	
## Syntax
The expression is:
	/ element [COMMA element ]*
	
It appears (optionally) at the end of an object definition, after any properties. No comma between the properties and the ordering expression is needed or allowed.
	
The "/" is chosen due to its mathematical connotation of division, which is worded as "by"; in this context it means: "order by". The specific token is used to disambiguate from a normal identifier in the object definition, where we want to avoid restricting the use of any word. So things like "OrderBy" as identifier were not introduced in the object language as keyword. Instead we selected the simple "/" as keyword to indicate the ordering list.

The ordering list is comma separated and may appear only at the end of the property definition. It is not preceded by a comma and only terminated by the "}" that terminates the object definition.
	
A complete (abstract) object definition thus looks as:
	
	ID (inherits ID)? : {
		(
			property (, property)? 
		)?
		(/ ID (, ID)*)?
	}
	
The line (/ ID (, ID)*)? defines how the ordering of the properties in this object will appear.
	
## Semantics
Every element  in the list must be present as a property of the object. If an element is not present in the object but present in the list of ordered properties, this will generate a warning, and that property is ignored in the ordering of properties.
Properties that are part of the object but not in the ordering list are appended at the end of the ordered list in the order they would be presented when the ordering would not be present.
	
	Examples:
	Basic:
	Obj : {
		Id : {},
		Code: {},
		Desc : {},
		A1: {},
		A2: {},
		B2: {}
		D3: {}
		/ ID, Desc, A2, D3
	}
	
Looping through this object will produce properties in this order:
	ID // ordered
	Desc // ordered
	A2 // ordered
	D3 // ordered
	Code // not ordered, remaining
	A1 // not ordered, remaining
	B2 // not ordered, remaining
	
Inheritance:
	Obj2 <- Obj : {
		+add: {},
		+add2: {},
		-D3
		/ ID, Desc, add, add2, A1, Code
	}
	Produces:
    ID, Desc, add, add2, A2, Code, A1, B2
	 
	 
