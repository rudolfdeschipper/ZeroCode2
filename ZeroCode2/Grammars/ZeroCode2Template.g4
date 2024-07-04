/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

parser grammar ZeroCode2Template;
// ------- PARSER ---------

options { tokenVocab=ZeroCode2TemplateLexer; }

template
	: templateelement+ EOF
	| EOF
	;

templateelement
	: command				#commandCommandIgnore
	| (TEXT | EQU | PERC )	#LiteralCommand
	;


command
	: filec					#filecCommandIgnore
	| fileo					#fileoCommandIgnore
	| include				#includeCommandIgnore
	| IF					#IfCommand
	| LOOP NEWLINE?			#LoopCommand
	| ENDFILE NEWLINE?		#EndFileCommand
	| ENDIF					#EndIfCommand
	| ELSE					#ElseCommand
	| ENDLOOP NEWLINE?		#EndLoopCommand
	| log					#LogCommand
	| EXPR					#ExprCommand
	| VAR					#VarCommand
;

filec
	: FILECREATE NEWLINE #FileCreateCommand
	;

fileo
	: FILEOVERWRITE NEWLINE #FileOverwriteCommand
	;

include
	: INCLUDE NEWLINE	#IncludeCommand
	;


log
	: INFO NEWLINE
    | DEBUG NEWLINE
    | LOG NEWLINE
    | ERROR NEWLINE
    | TRACE NEWLINE
    ;

