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
	;

templateelement
	: command			 #commandCommandIgnore
	| expr				 #exprCommandIgnore
	| (TEXT | EQU | WS ) #LiteralCommand
	;


command
	: filec			#filecCommandIgnore
	| fileo			#fileoCommandIgnore
	| include		#includeCommandIgnore
	| IF IFTEXT		#IfCommand
	| LOOP IGNORE	#LoopCommand
	| ENDFILE		#EndFileCommand
	| ENDIF			#EndIfCommand
	| ELSE			#ElseCommand
	| ENDLOOP		#EndLoopCommand
	| log			#LogCommand
	;

filec
	: FILEC (IGNORE)+ #FileCreateCommand
	;

fileo
	: FILEO (IGNORE)+ #FileOverwriteCommand
	;

include
	: INCLUDE (IGNORE)+  #IncludeCommand
	;

expr
	: EXPRO EXIGNORE EXPRC #ExprCommand
	;

log
	: INFO (IGNORE)+
	| DEBUG (IGNORE)+
	| LOG (IGNORE)+
	| ERROR (IGNORE)+
	| TRACE (IGNORE)+
	;
