using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace HelloWorldVSIX
{
    // we have to export the class in order
    // for it to replace the VS default functionality
    [Export(typeof(IIntellisensePresenterProvider))]
    [ContentType("XAML")] // let it work on XAML files
    [ContentType("CSharp")] // let it also work on the C# files
    [Order(Before = "default")] //it will be picked up before the VS default
    [Name("XAML Intellisense Extension")]
    public class HelloWorldIntellisenseProvider :
        // should implement IIntellisensePresenterProvider
        IIntellisensePresenterProvider
    {
        public IIntellisensePresenter 
            TryCreateIntellisensePresenter(IIntellisenseSession session)
        {

            // returning null will 
            // trigger the default Intellisense provider
            return new HelloWorldIntellisensePresenterControl(session as ICompletionSession);
        }
    }
}
