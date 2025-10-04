// Generated from c:/Users/pista/source/repos/ZeroCode2/ZeroCode2/Grammars/ZeroCode2Template.g4 by ANTLR 4.13.1
import org.antlr.v4.runtime.tree.ParseTreeListener;

/**
 * This interface defines a complete listener for a parse tree produced by
 * {@link ZeroCode2Template}.
 */
public interface ZeroCode2TemplateListener extends ParseTreeListener {
	/**
	 * Enter a parse tree produced by {@link ZeroCode2Template#template}.
	 * @param ctx the parse tree
	 */
	void enterTemplate(ZeroCode2Template.TemplateContext ctx);
	/**
	 * Exit a parse tree produced by {@link ZeroCode2Template#template}.
	 * @param ctx the parse tree
	 */
	void exitTemplate(ZeroCode2Template.TemplateContext ctx);
	/**
	 * Enter a parse tree produced by the {@code commandCommandIgnore}
	 * labeled alternative in {@link ZeroCode2Template#templateelement}.
	 * @param ctx the parse tree
	 */
	void enterCommandCommandIgnore(ZeroCode2Template.CommandCommandIgnoreContext ctx);
	/**
	 * Exit a parse tree produced by the {@code commandCommandIgnore}
	 * labeled alternative in {@link ZeroCode2Template#templateelement}.
	 * @param ctx the parse tree
	 */
	void exitCommandCommandIgnore(ZeroCode2Template.CommandCommandIgnoreContext ctx);
	/**
	 * Enter a parse tree produced by the {@code LiteralCommand}
	 * labeled alternative in {@link ZeroCode2Template#templateelement}.
	 * @param ctx the parse tree
	 */
	void enterLiteralCommand(ZeroCode2Template.LiteralCommandContext ctx);
	/**
	 * Exit a parse tree produced by the {@code LiteralCommand}
	 * labeled alternative in {@link ZeroCode2Template#templateelement}.
	 * @param ctx the parse tree
	 */
	void exitLiteralCommand(ZeroCode2Template.LiteralCommandContext ctx);
	/**
	 * Enter a parse tree produced by the {@code filecCommandIgnore}
	 * labeled alternative in {@link ZeroCode2Template#command}.
	 * @param ctx the parse tree
	 */
	void enterFilecCommandIgnore(ZeroCode2Template.FilecCommandIgnoreContext ctx);
	/**
	 * Exit a parse tree produced by the {@code filecCommandIgnore}
	 * labeled alternative in {@link ZeroCode2Template#command}.
	 * @param ctx the parse tree
	 */
	void exitFilecCommandIgnore(ZeroCode2Template.FilecCommandIgnoreContext ctx);
	/**
	 * Enter a parse tree produced by the {@code fileoCommandIgnore}
	 * labeled alternative in {@link ZeroCode2Template#command}.
	 * @param ctx the parse tree
	 */
	void enterFileoCommandIgnore(ZeroCode2Template.FileoCommandIgnoreContext ctx);
	/**
	 * Exit a parse tree produced by the {@code fileoCommandIgnore}
	 * labeled alternative in {@link ZeroCode2Template#command}.
	 * @param ctx the parse tree
	 */
	void exitFileoCommandIgnore(ZeroCode2Template.FileoCommandIgnoreContext ctx);
	/**
	 * Enter a parse tree produced by the {@code includeCommandIgnore}
	 * labeled alternative in {@link ZeroCode2Template#command}.
	 * @param ctx the parse tree
	 */
	void enterIncludeCommandIgnore(ZeroCode2Template.IncludeCommandIgnoreContext ctx);
	/**
	 * Exit a parse tree produced by the {@code includeCommandIgnore}
	 * labeled alternative in {@link ZeroCode2Template#command}.
	 * @param ctx the parse tree
	 */
	void exitIncludeCommandIgnore(ZeroCode2Template.IncludeCommandIgnoreContext ctx);
	/**
	 * Enter a parse tree produced by the {@code IfCommand}
	 * labeled alternative in {@link ZeroCode2Template#command}.
	 * @param ctx the parse tree
	 */
	void enterIfCommand(ZeroCode2Template.IfCommandContext ctx);
	/**
	 * Exit a parse tree produced by the {@code IfCommand}
	 * labeled alternative in {@link ZeroCode2Template#command}.
	 * @param ctx the parse tree
	 */
	void exitIfCommand(ZeroCode2Template.IfCommandContext ctx);
	/**
	 * Enter a parse tree produced by the {@code LoopCommand}
	 * labeled alternative in {@link ZeroCode2Template#command}.
	 * @param ctx the parse tree
	 */
	void enterLoopCommand(ZeroCode2Template.LoopCommandContext ctx);
	/**
	 * Exit a parse tree produced by the {@code LoopCommand}
	 * labeled alternative in {@link ZeroCode2Template#command}.
	 * @param ctx the parse tree
	 */
	void exitLoopCommand(ZeroCode2Template.LoopCommandContext ctx);
	/**
	 * Enter a parse tree produced by the {@code EndFileCommand}
	 * labeled alternative in {@link ZeroCode2Template#command}.
	 * @param ctx the parse tree
	 */
	void enterEndFileCommand(ZeroCode2Template.EndFileCommandContext ctx);
	/**
	 * Exit a parse tree produced by the {@code EndFileCommand}
	 * labeled alternative in {@link ZeroCode2Template#command}.
	 * @param ctx the parse tree
	 */
	void exitEndFileCommand(ZeroCode2Template.EndFileCommandContext ctx);
	/**
	 * Enter a parse tree produced by the {@code EndIfCommand}
	 * labeled alternative in {@link ZeroCode2Template#command}.
	 * @param ctx the parse tree
	 */
	void enterEndIfCommand(ZeroCode2Template.EndIfCommandContext ctx);
	/**
	 * Exit a parse tree produced by the {@code EndIfCommand}
	 * labeled alternative in {@link ZeroCode2Template#command}.
	 * @param ctx the parse tree
	 */
	void exitEndIfCommand(ZeroCode2Template.EndIfCommandContext ctx);
	/**
	 * Enter a parse tree produced by the {@code ElseCommand}
	 * labeled alternative in {@link ZeroCode2Template#command}.
	 * @param ctx the parse tree
	 */
	void enterElseCommand(ZeroCode2Template.ElseCommandContext ctx);
	/**
	 * Exit a parse tree produced by the {@code ElseCommand}
	 * labeled alternative in {@link ZeroCode2Template#command}.
	 * @param ctx the parse tree
	 */
	void exitElseCommand(ZeroCode2Template.ElseCommandContext ctx);
	/**
	 * Enter a parse tree produced by the {@code EndLoopCommand}
	 * labeled alternative in {@link ZeroCode2Template#command}.
	 * @param ctx the parse tree
	 */
	void enterEndLoopCommand(ZeroCode2Template.EndLoopCommandContext ctx);
	/**
	 * Exit a parse tree produced by the {@code EndLoopCommand}
	 * labeled alternative in {@link ZeroCode2Template#command}.
	 * @param ctx the parse tree
	 */
	void exitEndLoopCommand(ZeroCode2Template.EndLoopCommandContext ctx);
	/**
	 * Enter a parse tree produced by the {@code LogCommand}
	 * labeled alternative in {@link ZeroCode2Template#command}.
	 * @param ctx the parse tree
	 */
	void enterLogCommand(ZeroCode2Template.LogCommandContext ctx);
	/**
	 * Exit a parse tree produced by the {@code LogCommand}
	 * labeled alternative in {@link ZeroCode2Template#command}.
	 * @param ctx the parse tree
	 */
	void exitLogCommand(ZeroCode2Template.LogCommandContext ctx);
	/**
	 * Enter a parse tree produced by the {@code ExprCommand}
	 * labeled alternative in {@link ZeroCode2Template#command}.
	 * @param ctx the parse tree
	 */
	void enterExprCommand(ZeroCode2Template.ExprCommandContext ctx);
	/**
	 * Exit a parse tree produced by the {@code ExprCommand}
	 * labeled alternative in {@link ZeroCode2Template#command}.
	 * @param ctx the parse tree
	 */
	void exitExprCommand(ZeroCode2Template.ExprCommandContext ctx);
	/**
	 * Enter a parse tree produced by the {@code VarCommand}
	 * labeled alternative in {@link ZeroCode2Template#command}.
	 * @param ctx the parse tree
	 */
	void enterVarCommand(ZeroCode2Template.VarCommandContext ctx);
	/**
	 * Exit a parse tree produced by the {@code VarCommand}
	 * labeled alternative in {@link ZeroCode2Template#command}.
	 * @param ctx the parse tree
	 */
	void exitVarCommand(ZeroCode2Template.VarCommandContext ctx);
	/**
	 * Enter a parse tree produced by the {@code FileCreateCommand}
	 * labeled alternative in {@link ZeroCode2Template#filec}.
	 * @param ctx the parse tree
	 */
	void enterFileCreateCommand(ZeroCode2Template.FileCreateCommandContext ctx);
	/**
	 * Exit a parse tree produced by the {@code FileCreateCommand}
	 * labeled alternative in {@link ZeroCode2Template#filec}.
	 * @param ctx the parse tree
	 */
	void exitFileCreateCommand(ZeroCode2Template.FileCreateCommandContext ctx);
	/**
	 * Enter a parse tree produced by the {@code FileOverwriteCommand}
	 * labeled alternative in {@link ZeroCode2Template#fileo}.
	 * @param ctx the parse tree
	 */
	void enterFileOverwriteCommand(ZeroCode2Template.FileOverwriteCommandContext ctx);
	/**
	 * Exit a parse tree produced by the {@code FileOverwriteCommand}
	 * labeled alternative in {@link ZeroCode2Template#fileo}.
	 * @param ctx the parse tree
	 */
	void exitFileOverwriteCommand(ZeroCode2Template.FileOverwriteCommandContext ctx);
	/**
	 * Enter a parse tree produced by the {@code IncludeCommand}
	 * labeled alternative in {@link ZeroCode2Template#include}.
	 * @param ctx the parse tree
	 */
	void enterIncludeCommand(ZeroCode2Template.IncludeCommandContext ctx);
	/**
	 * Exit a parse tree produced by the {@code IncludeCommand}
	 * labeled alternative in {@link ZeroCode2Template#include}.
	 * @param ctx the parse tree
	 */
	void exitIncludeCommand(ZeroCode2Template.IncludeCommandContext ctx);
	/**
	 * Enter a parse tree produced by {@link ZeroCode2Template#log}.
	 * @param ctx the parse tree
	 */
	void enterLog(ZeroCode2Template.LogContext ctx);
	/**
	 * Exit a parse tree produced by {@link ZeroCode2Template#log}.
	 * @param ctx the parse tree
	 */
	void exitLog(ZeroCode2Template.LogContext ctx);
}