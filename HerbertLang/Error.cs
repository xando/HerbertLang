using System;

namespace HerberLang {

    // TODO: 
    // We can change errors to not have strings being passed in the consturctor.
    // Just define them here as format. This should help with testing. 

    public class LanguageError : Exception {
        public int line = 0;
        public int column = 0;
        public LanguageError(string message)
        : base(message) { }

        public LanguageError(string message, int line, int column) : base(message) {
            this.line = line;
            this.column = column;
        }

        public LanguageError(string message, INode node) : base(message) {
            this.line = node.line;
            this.column = node.column;
        }
        
        public override string ToString() {
            return string.Format("{0}:{1} {2}", line, column, Message);
        }
    }
}