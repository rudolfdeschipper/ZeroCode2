// Generated from c:/Users/pista/source/repos/ZeroCode2/ZeroCode2/Grammars/ZeroCode2Template.g4 by ANTLR 4.13.1
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.misc.*;
import org.antlr.v4.runtime.tree.*;
import java.util.List;
import java.util.Iterator;
import java.util.ArrayList;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast", "CheckReturnValue"})
public class ZeroCode2Template extends Parser {
	static { RuntimeMetaData.checkVersion("4.13.1", RuntimeMetaData.VERSION); }

	protected static final DFA[] _decisionToDFA;
	protected static final PredictionContextCache _sharedContextCache =
		new PredictionContextCache();
	public static final int
		FILEOVERWRITE=1, FILECREATE=2, INCLUDE=3, ENDFILE=4, LOOP=5, ENDLOOP=6, 
		IF=7, ELSE=8, ENDIF=9, EXPR=10, VAR=11, INFO=12, DEBUG=13, ERROR=14, LOG=15, 
		TRACE=16, EQU=17, PERC=18, TEXT=19, NEWLINE=20;
	public static final int
		RULE_template = 0, RULE_templateelement = 1, RULE_command = 2, RULE_filec = 3, 
		RULE_fileo = 4, RULE_include = 5, RULE_log = 6;
	private static String[] makeRuleNames() {
		return new String[] {
			"template", "templateelement", "command", "filec", "fileo", "include", 
			"log"
		};
	}
	public static final String[] ruleNames = makeRuleNames();

	private static String[] makeLiteralNames() {
		return new String[] {
			null, null, null, null, "'%/File'", null, null, null, null, null, null, 
			null, null, null, null, null, null, "'='", "'%'"
		};
	}
	private static final String[] _LITERAL_NAMES = makeLiteralNames();
	private static String[] makeSymbolicNames() {
		return new String[] {
			null, "FILEOVERWRITE", "FILECREATE", "INCLUDE", "ENDFILE", "LOOP", "ENDLOOP", 
			"IF", "ELSE", "ENDIF", "EXPR", "VAR", "INFO", "DEBUG", "ERROR", "LOG", 
			"TRACE", "EQU", "PERC", "TEXT", "NEWLINE"
		};
	}
	private static final String[] _SYMBOLIC_NAMES = makeSymbolicNames();
	public static final Vocabulary VOCABULARY = new VocabularyImpl(_LITERAL_NAMES, _SYMBOLIC_NAMES);

	/**
	 * @deprecated Use {@link #VOCABULARY} instead.
	 */
	@Deprecated
	public static final String[] tokenNames;
	static {
		tokenNames = new String[_SYMBOLIC_NAMES.length];
		for (int i = 0; i < tokenNames.length; i++) {
			tokenNames[i] = VOCABULARY.getLiteralName(i);
			if (tokenNames[i] == null) {
				tokenNames[i] = VOCABULARY.getSymbolicName(i);
			}

			if (tokenNames[i] == null) {
				tokenNames[i] = "<INVALID>";
			}
		}
	}

	@Override
	@Deprecated
	public String[] getTokenNames() {
		return tokenNames;
	}

	@Override

	public Vocabulary getVocabulary() {
		return VOCABULARY;
	}

	@Override
	public String getGrammarFileName() { return "ZeroCode2Template.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public ATN getATN() { return _ATN; }

	public ZeroCode2Template(TokenStream input) {
		super(input);
		_interp = new ParserATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}

	@SuppressWarnings("CheckReturnValue")
	public static class TemplateContext extends ParserRuleContext {
		public TerminalNode EOF() { return getToken(ZeroCode2Template.EOF, 0); }
		public List<TemplateelementContext> templateelement() {
			return getRuleContexts(TemplateelementContext.class);
		}
		public TemplateelementContext templateelement(int i) {
			return getRuleContext(TemplateelementContext.class,i);
		}
		public TemplateContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_template; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).enterTemplate(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).exitTemplate(this);
		}
	}

	public final TemplateContext template() throws RecognitionException {
		TemplateContext _localctx = new TemplateContext(_ctx, getState());
		enterRule(_localctx, 0, RULE_template);
		int _la;
		try {
			setState(22);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case FILEOVERWRITE:
			case FILECREATE:
			case INCLUDE:
			case ENDFILE:
			case LOOP:
			case ENDLOOP:
			case IF:
			case ELSE:
			case ENDIF:
			case EXPR:
			case VAR:
			case INFO:
			case DEBUG:
			case ERROR:
			case LOG:
			case TRACE:
			case EQU:
			case PERC:
			case TEXT:
				enterOuterAlt(_localctx, 1);
				{
				setState(15); 
				_errHandler.sync(this);
				_la = _input.LA(1);
				do {
					{
					{
					setState(14);
					templateelement();
					}
					}
					setState(17); 
					_errHandler.sync(this);
					_la = _input.LA(1);
				} while ( (((_la) & ~0x3f) == 0 && ((1L << _la) & 1048574L) != 0) );
				setState(19);
				match(EOF);
				}
				break;
			case EOF:
				enterOuterAlt(_localctx, 2);
				{
				setState(21);
				match(EOF);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class TemplateelementContext extends ParserRuleContext {
		public TemplateelementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_templateelement; }
	 
		public TemplateelementContext() { }
		public void copyFrom(TemplateelementContext ctx) {
			super.copyFrom(ctx);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class LiteralCommandContext extends TemplateelementContext {
		public TerminalNode TEXT() { return getToken(ZeroCode2Template.TEXT, 0); }
		public TerminalNode EQU() { return getToken(ZeroCode2Template.EQU, 0); }
		public TerminalNode PERC() { return getToken(ZeroCode2Template.PERC, 0); }
		public LiteralCommandContext(TemplateelementContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).enterLiteralCommand(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).exitLiteralCommand(this);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class CommandCommandIgnoreContext extends TemplateelementContext {
		public CommandContext command() {
			return getRuleContext(CommandContext.class,0);
		}
		public CommandCommandIgnoreContext(TemplateelementContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).enterCommandCommandIgnore(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).exitCommandCommandIgnore(this);
		}
	}

	public final TemplateelementContext templateelement() throws RecognitionException {
		TemplateelementContext _localctx = new TemplateelementContext(_ctx, getState());
		enterRule(_localctx, 2, RULE_templateelement);
		int _la;
		try {
			setState(26);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case FILEOVERWRITE:
			case FILECREATE:
			case INCLUDE:
			case ENDFILE:
			case LOOP:
			case ENDLOOP:
			case IF:
			case ELSE:
			case ENDIF:
			case EXPR:
			case VAR:
			case INFO:
			case DEBUG:
			case ERROR:
			case LOG:
			case TRACE:
				_localctx = new CommandCommandIgnoreContext(_localctx);
				enterOuterAlt(_localctx, 1);
				{
				setState(24);
				command();
				}
				break;
			case EQU:
			case PERC:
			case TEXT:
				_localctx = new LiteralCommandContext(_localctx);
				enterOuterAlt(_localctx, 2);
				{
				setState(25);
				_la = _input.LA(1);
				if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & 917504L) != 0)) ) {
				_errHandler.recoverInline(this);
				}
				else {
					if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
					_errHandler.reportMatch(this);
					consume();
				}
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class CommandContext extends ParserRuleContext {
		public CommandContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_command; }
	 
		public CommandContext() { }
		public void copyFrom(CommandContext ctx) {
			super.copyFrom(ctx);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class ElseCommandContext extends CommandContext {
		public TerminalNode ELSE() { return getToken(ZeroCode2Template.ELSE, 0); }
		public ElseCommandContext(CommandContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).enterElseCommand(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).exitElseCommand(this);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class ExprCommandContext extends CommandContext {
		public TerminalNode EXPR() { return getToken(ZeroCode2Template.EXPR, 0); }
		public ExprCommandContext(CommandContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).enterExprCommand(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).exitExprCommand(this);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class EndFileCommandContext extends CommandContext {
		public TerminalNode ENDFILE() { return getToken(ZeroCode2Template.ENDFILE, 0); }
		public TerminalNode NEWLINE() { return getToken(ZeroCode2Template.NEWLINE, 0); }
		public EndFileCommandContext(CommandContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).enterEndFileCommand(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).exitEndFileCommand(this);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class EndIfCommandContext extends CommandContext {
		public TerminalNode ENDIF() { return getToken(ZeroCode2Template.ENDIF, 0); }
		public EndIfCommandContext(CommandContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).enterEndIfCommand(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).exitEndIfCommand(this);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class IncludeCommandIgnoreContext extends CommandContext {
		public IncludeContext include() {
			return getRuleContext(IncludeContext.class,0);
		}
		public IncludeCommandIgnoreContext(CommandContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).enterIncludeCommandIgnore(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).exitIncludeCommandIgnore(this);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class LogCommandContext extends CommandContext {
		public LogContext log() {
			return getRuleContext(LogContext.class,0);
		}
		public LogCommandContext(CommandContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).enterLogCommand(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).exitLogCommand(this);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class FilecCommandIgnoreContext extends CommandContext {
		public FilecContext filec() {
			return getRuleContext(FilecContext.class,0);
		}
		public FilecCommandIgnoreContext(CommandContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).enterFilecCommandIgnore(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).exitFilecCommandIgnore(this);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class IfCommandContext extends CommandContext {
		public TerminalNode IF() { return getToken(ZeroCode2Template.IF, 0); }
		public IfCommandContext(CommandContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).enterIfCommand(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).exitIfCommand(this);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class EndLoopCommandContext extends CommandContext {
		public TerminalNode ENDLOOP() { return getToken(ZeroCode2Template.ENDLOOP, 0); }
		public TerminalNode NEWLINE() { return getToken(ZeroCode2Template.NEWLINE, 0); }
		public EndLoopCommandContext(CommandContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).enterEndLoopCommand(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).exitEndLoopCommand(this);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class VarCommandContext extends CommandContext {
		public TerminalNode VAR() { return getToken(ZeroCode2Template.VAR, 0); }
		public VarCommandContext(CommandContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).enterVarCommand(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).exitVarCommand(this);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class LoopCommandContext extends CommandContext {
		public TerminalNode LOOP() { return getToken(ZeroCode2Template.LOOP, 0); }
		public TerminalNode NEWLINE() { return getToken(ZeroCode2Template.NEWLINE, 0); }
		public LoopCommandContext(CommandContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).enterLoopCommand(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).exitLoopCommand(this);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class FileoCommandIgnoreContext extends CommandContext {
		public FileoContext fileo() {
			return getRuleContext(FileoContext.class,0);
		}
		public FileoCommandIgnoreContext(CommandContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).enterFileoCommandIgnore(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).exitFileoCommandIgnore(this);
		}
	}

	public final CommandContext command() throws RecognitionException {
		CommandContext _localctx = new CommandContext(_ctx, getState());
		enterRule(_localctx, 4, RULE_command);
		int _la;
		try {
			setState(49);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case FILECREATE:
				_localctx = new FilecCommandIgnoreContext(_localctx);
				enterOuterAlt(_localctx, 1);
				{
				setState(28);
				filec();
				}
				break;
			case FILEOVERWRITE:
				_localctx = new FileoCommandIgnoreContext(_localctx);
				enterOuterAlt(_localctx, 2);
				{
				setState(29);
				fileo();
				}
				break;
			case INCLUDE:
				_localctx = new IncludeCommandIgnoreContext(_localctx);
				enterOuterAlt(_localctx, 3);
				{
				setState(30);
				include();
				}
				break;
			case IF:
				_localctx = new IfCommandContext(_localctx);
				enterOuterAlt(_localctx, 4);
				{
				setState(31);
				match(IF);
				}
				break;
			case LOOP:
				_localctx = new LoopCommandContext(_localctx);
				enterOuterAlt(_localctx, 5);
				{
				setState(32);
				match(LOOP);
				setState(34);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==NEWLINE) {
					{
					setState(33);
					match(NEWLINE);
					}
				}

				}
				break;
			case ENDFILE:
				_localctx = new EndFileCommandContext(_localctx);
				enterOuterAlt(_localctx, 6);
				{
				setState(36);
				match(ENDFILE);
				setState(38);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==NEWLINE) {
					{
					setState(37);
					match(NEWLINE);
					}
				}

				}
				break;
			case ENDIF:
				_localctx = new EndIfCommandContext(_localctx);
				enterOuterAlt(_localctx, 7);
				{
				setState(40);
				match(ENDIF);
				}
				break;
			case ELSE:
				_localctx = new ElseCommandContext(_localctx);
				enterOuterAlt(_localctx, 8);
				{
				setState(41);
				match(ELSE);
				}
				break;
			case ENDLOOP:
				_localctx = new EndLoopCommandContext(_localctx);
				enterOuterAlt(_localctx, 9);
				{
				setState(42);
				match(ENDLOOP);
				setState(44);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==NEWLINE) {
					{
					setState(43);
					match(NEWLINE);
					}
				}

				}
				break;
			case INFO:
			case DEBUG:
			case ERROR:
			case LOG:
			case TRACE:
				_localctx = new LogCommandContext(_localctx);
				enterOuterAlt(_localctx, 10);
				{
				setState(46);
				log();
				}
				break;
			case EXPR:
				_localctx = new ExprCommandContext(_localctx);
				enterOuterAlt(_localctx, 11);
				{
				setState(47);
				match(EXPR);
				}
				break;
			case VAR:
				_localctx = new VarCommandContext(_localctx);
				enterOuterAlt(_localctx, 12);
				{
				setState(48);
				match(VAR);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class FilecContext extends ParserRuleContext {
		public FilecContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_filec; }
	 
		public FilecContext() { }
		public void copyFrom(FilecContext ctx) {
			super.copyFrom(ctx);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class FileCreateCommandContext extends FilecContext {
		public TerminalNode FILECREATE() { return getToken(ZeroCode2Template.FILECREATE, 0); }
		public TerminalNode NEWLINE() { return getToken(ZeroCode2Template.NEWLINE, 0); }
		public FileCreateCommandContext(FilecContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).enterFileCreateCommand(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).exitFileCreateCommand(this);
		}
	}

	public final FilecContext filec() throws RecognitionException {
		FilecContext _localctx = new FilecContext(_ctx, getState());
		enterRule(_localctx, 6, RULE_filec);
		try {
			_localctx = new FileCreateCommandContext(_localctx);
			enterOuterAlt(_localctx, 1);
			{
			setState(51);
			match(FILECREATE);
			setState(52);
			match(NEWLINE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class FileoContext extends ParserRuleContext {
		public FileoContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_fileo; }
	 
		public FileoContext() { }
		public void copyFrom(FileoContext ctx) {
			super.copyFrom(ctx);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class FileOverwriteCommandContext extends FileoContext {
		public TerminalNode FILEOVERWRITE() { return getToken(ZeroCode2Template.FILEOVERWRITE, 0); }
		public TerminalNode NEWLINE() { return getToken(ZeroCode2Template.NEWLINE, 0); }
		public FileOverwriteCommandContext(FileoContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).enterFileOverwriteCommand(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).exitFileOverwriteCommand(this);
		}
	}

	public final FileoContext fileo() throws RecognitionException {
		FileoContext _localctx = new FileoContext(_ctx, getState());
		enterRule(_localctx, 8, RULE_fileo);
		try {
			_localctx = new FileOverwriteCommandContext(_localctx);
			enterOuterAlt(_localctx, 1);
			{
			setState(54);
			match(FILEOVERWRITE);
			setState(55);
			match(NEWLINE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class IncludeContext extends ParserRuleContext {
		public IncludeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_include; }
	 
		public IncludeContext() { }
		public void copyFrom(IncludeContext ctx) {
			super.copyFrom(ctx);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class IncludeCommandContext extends IncludeContext {
		public TerminalNode INCLUDE() { return getToken(ZeroCode2Template.INCLUDE, 0); }
		public TerminalNode NEWLINE() { return getToken(ZeroCode2Template.NEWLINE, 0); }
		public IncludeCommandContext(IncludeContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).enterIncludeCommand(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).exitIncludeCommand(this);
		}
	}

	public final IncludeContext include() throws RecognitionException {
		IncludeContext _localctx = new IncludeContext(_ctx, getState());
		enterRule(_localctx, 10, RULE_include);
		try {
			_localctx = new IncludeCommandContext(_localctx);
			enterOuterAlt(_localctx, 1);
			{
			setState(57);
			match(INCLUDE);
			setState(58);
			match(NEWLINE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class LogContext extends ParserRuleContext {
		public TerminalNode INFO() { return getToken(ZeroCode2Template.INFO, 0); }
		public TerminalNode NEWLINE() { return getToken(ZeroCode2Template.NEWLINE, 0); }
		public TerminalNode DEBUG() { return getToken(ZeroCode2Template.DEBUG, 0); }
		public TerminalNode LOG() { return getToken(ZeroCode2Template.LOG, 0); }
		public TerminalNode ERROR() { return getToken(ZeroCode2Template.ERROR, 0); }
		public TerminalNode TRACE() { return getToken(ZeroCode2Template.TRACE, 0); }
		public LogContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_log; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).enterLog(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof ZeroCode2TemplateListener ) ((ZeroCode2TemplateListener)listener).exitLog(this);
		}
	}

	public final LogContext log() throws RecognitionException {
		LogContext _localctx = new LogContext(_ctx, getState());
		enterRule(_localctx, 12, RULE_log);
		try {
			setState(70);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case INFO:
				enterOuterAlt(_localctx, 1);
				{
				setState(60);
				match(INFO);
				setState(61);
				match(NEWLINE);
				}
				break;
			case DEBUG:
				enterOuterAlt(_localctx, 2);
				{
				setState(62);
				match(DEBUG);
				setState(63);
				match(NEWLINE);
				}
				break;
			case LOG:
				enterOuterAlt(_localctx, 3);
				{
				setState(64);
				match(LOG);
				setState(65);
				match(NEWLINE);
				}
				break;
			case ERROR:
				enterOuterAlt(_localctx, 4);
				{
				setState(66);
				match(ERROR);
				setState(67);
				match(NEWLINE);
				}
				break;
			case TRACE:
				enterOuterAlt(_localctx, 5);
				{
				setState(68);
				match(TRACE);
				setState(69);
				match(NEWLINE);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static final String _serializedATN =
		"\u0004\u0001\u0014I\u0002\u0000\u0007\u0000\u0002\u0001\u0007\u0001\u0002"+
		"\u0002\u0007\u0002\u0002\u0003\u0007\u0003\u0002\u0004\u0007\u0004\u0002"+
		"\u0005\u0007\u0005\u0002\u0006\u0007\u0006\u0001\u0000\u0004\u0000\u0010"+
		"\b\u0000\u000b\u0000\f\u0000\u0011\u0001\u0000\u0001\u0000\u0001\u0000"+
		"\u0003\u0000\u0017\b\u0000\u0001\u0001\u0001\u0001\u0003\u0001\u001b\b"+
		"\u0001\u0001\u0002\u0001\u0002\u0001\u0002\u0001\u0002\u0001\u0002\u0001"+
		"\u0002\u0003\u0002#\b\u0002\u0001\u0002\u0001\u0002\u0003\u0002\'\b\u0002"+
		"\u0001\u0002\u0001\u0002\u0001\u0002\u0001\u0002\u0003\u0002-\b\u0002"+
		"\u0001\u0002\u0001\u0002\u0001\u0002\u0003\u00022\b\u0002\u0001\u0003"+
		"\u0001\u0003\u0001\u0003\u0001\u0004\u0001\u0004\u0001\u0004\u0001\u0005"+
		"\u0001\u0005\u0001\u0005\u0001\u0006\u0001\u0006\u0001\u0006\u0001\u0006"+
		"\u0001\u0006\u0001\u0006\u0001\u0006\u0001\u0006\u0001\u0006\u0001\u0006"+
		"\u0003\u0006G\b\u0006\u0001\u0006\u0000\u0000\u0007\u0000\u0002\u0004"+
		"\u0006\b\n\f\u0000\u0001\u0001\u0000\u0011\u0013V\u0000\u0016\u0001\u0000"+
		"\u0000\u0000\u0002\u001a\u0001\u0000\u0000\u0000\u00041\u0001\u0000\u0000"+
		"\u0000\u00063\u0001\u0000\u0000\u0000\b6\u0001\u0000\u0000\u0000\n9\u0001"+
		"\u0000\u0000\u0000\fF\u0001\u0000\u0000\u0000\u000e\u0010\u0003\u0002"+
		"\u0001\u0000\u000f\u000e\u0001\u0000\u0000\u0000\u0010\u0011\u0001\u0000"+
		"\u0000\u0000\u0011\u000f\u0001\u0000\u0000\u0000\u0011\u0012\u0001\u0000"+
		"\u0000\u0000\u0012\u0013\u0001\u0000\u0000\u0000\u0013\u0014\u0005\u0000"+
		"\u0000\u0001\u0014\u0017\u0001\u0000\u0000\u0000\u0015\u0017\u0005\u0000"+
		"\u0000\u0001\u0016\u000f\u0001\u0000\u0000\u0000\u0016\u0015\u0001\u0000"+
		"\u0000\u0000\u0017\u0001\u0001\u0000\u0000\u0000\u0018\u001b\u0003\u0004"+
		"\u0002\u0000\u0019\u001b\u0007\u0000\u0000\u0000\u001a\u0018\u0001\u0000"+
		"\u0000\u0000\u001a\u0019\u0001\u0000\u0000\u0000\u001b\u0003\u0001\u0000"+
		"\u0000\u0000\u001c2\u0003\u0006\u0003\u0000\u001d2\u0003\b\u0004\u0000"+
		"\u001e2\u0003\n\u0005\u0000\u001f2\u0005\u0007\u0000\u0000 \"\u0005\u0005"+
		"\u0000\u0000!#\u0005\u0014\u0000\u0000\"!\u0001\u0000\u0000\u0000\"#\u0001"+
		"\u0000\u0000\u0000#2\u0001\u0000\u0000\u0000$&\u0005\u0004\u0000\u0000"+
		"%\'\u0005\u0014\u0000\u0000&%\u0001\u0000\u0000\u0000&\'\u0001\u0000\u0000"+
		"\u0000\'2\u0001\u0000\u0000\u0000(2\u0005\t\u0000\u0000)2\u0005\b\u0000"+
		"\u0000*,\u0005\u0006\u0000\u0000+-\u0005\u0014\u0000\u0000,+\u0001\u0000"+
		"\u0000\u0000,-\u0001\u0000\u0000\u0000-2\u0001\u0000\u0000\u0000.2\u0003"+
		"\f\u0006\u0000/2\u0005\n\u0000\u000002\u0005\u000b\u0000\u00001\u001c"+
		"\u0001\u0000\u0000\u00001\u001d\u0001\u0000\u0000\u00001\u001e\u0001\u0000"+
		"\u0000\u00001\u001f\u0001\u0000\u0000\u00001 \u0001\u0000\u0000\u0000"+
		"1$\u0001\u0000\u0000\u00001(\u0001\u0000\u0000\u00001)\u0001\u0000\u0000"+
		"\u00001*\u0001\u0000\u0000\u00001.\u0001\u0000\u0000\u00001/\u0001\u0000"+
		"\u0000\u000010\u0001\u0000\u0000\u00002\u0005\u0001\u0000\u0000\u0000"+
		"34\u0005\u0002\u0000\u000045\u0005\u0014\u0000\u00005\u0007\u0001\u0000"+
		"\u0000\u000067\u0005\u0001\u0000\u000078\u0005\u0014\u0000\u00008\t\u0001"+
		"\u0000\u0000\u00009:\u0005\u0003\u0000\u0000:;\u0005\u0014\u0000\u0000"+
		";\u000b\u0001\u0000\u0000\u0000<=\u0005\f\u0000\u0000=G\u0005\u0014\u0000"+
		"\u0000>?\u0005\r\u0000\u0000?G\u0005\u0014\u0000\u0000@A\u0005\u000f\u0000"+
		"\u0000AG\u0005\u0014\u0000\u0000BC\u0005\u000e\u0000\u0000CG\u0005\u0014"+
		"\u0000\u0000DE\u0005\u0010\u0000\u0000EG\u0005\u0014\u0000\u0000F<\u0001"+
		"\u0000\u0000\u0000F>\u0001\u0000\u0000\u0000F@\u0001\u0000\u0000\u0000"+
		"FB\u0001\u0000\u0000\u0000FD\u0001\u0000\u0000\u0000G\r\u0001\u0000\u0000"+
		"\u0000\b\u0011\u0016\u001a\"&,1F";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}