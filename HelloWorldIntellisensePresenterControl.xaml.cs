using Microsoft.VisualStudio.Language.Intellisense;
using System.Windows.Controls;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using System;
using System.Windows;
using Microsoft.VisualStudio.Text.Editor;
using System.Collections.Generic;
using System.Linq;

namespace HelloWorldVSIX
{
    /// <summary>
    /// Interaction logic for HelloWorldIntellisensePresenterControl.xaml
    /// </summary>
    public partial class HelloWorldIntellisensePresenterControl :
        UserControl,
        IPopupIntellisensePresenter
    {
        #region IPopupIntellisensePresenter IMPLEMENTATION

        // returns itself as a popup
        public UIElement SurfaceElement => this;

        // determines where the intellisense popup is 
        // being displayed
        public ITrackingSpan PresentationSpan
        {
            get
            {
                SnapshotSpan span =
                    this.CompletionSession
                            .SelectedCompletionSet
                            .ApplicableTo
                            .GetSpan
                            (
                                this.CompletionSession.TextView.TextSnapshot
                            );

                NormalizedSnapshotSpanCollection spans =
                    this.CompletionSession
                            .TextView
                            .BufferGraph
                            .MapUpToBuffer
                            (
                                span,
                                this.CompletionSession
                                        .SelectedCompletionSet
                                        .ApplicableTo
                                        .TrackingMode,
                                this.CompletionSession.TextView.TextBuffer);
                if (spans.Count <= 0)
                {
                    throw new InvalidOperationException
                    (
                        @"Completion Session Applicable-To Span is invalid.  
It doesn't map to a span in the session's text view."
                    );
                }
                SnapshotSpan span2 = spans[0];

                return
                    this.CompletionSession
                            .TextView
                            .TextBuffer
                            .CurrentSnapshot
                            .CreateTrackingSpan(span2.Span, SpanTrackingMode.EdgeInclusive);
            }
        }

        public PopupStyles PopupStyles => 
            PopupStyles.PositionClosest;

        // for some reason it should 
        // always be set to "completion".
        // Otherwise the popup will not popup
        public string SpaceReservationManagerName =>
            "completion";

        // intellisense completion session
        public IIntellisenseSession Session =>
            CompletionSession;

#pragma warning disable 0067
        public event EventHandler SurfaceElementChanged;
        public event EventHandler PresentationSpanChanged;
        public event EventHandler<ValueChangedEventArgs<PopupStyles>> PopupStylesChanged;
#pragma warning restore 0067
        #endregion IPopupIntellisensePresenter IMPLEMENTATION

        // interllisense session which is set in 
        // the constructor
        ICompletionSession CompletionSession { get; }

        public Completion TheFirstCompletion { get; }

        public HelloWorldIntellisensePresenterControl
        (
            ICompletionSession completionSession
        )
        {
            this.CompletionSession = completionSession;

            // getting all possible completions
            List<Completion> allCompletions =
                this.CompletionSession
                    .SelectedCompletionSet
                    .Completions
                    .ToList();

            // we choose the fist completion
            // out all possible completions
            TheFirstCompletion = allCompletions.First();

            InitializeComponent();

            // set the text of the popup
            // to the text to be inserted
            // in case the user clicks "Commit"
            FirstCompletionText.Text = TheFirstCompletion.InsertionText;

            CommitButton.Click += CommitButton_Click;
            DismissButton.Click += DismissButton_Click;
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            // set the selection status
            CompletionSession.SelectedCompletionSet.SelectionStatus =
                new CompletionSelectionStatus
                (
                    TheFirstCompletion,
                    true,
                    true
                );

            // commit to set the change
            CompletionSession.Commit();
        }

        private void DismissButton_Click(object sender, RoutedEventArgs e)
        {
            // even though the selection status is set
            // calling CompletionSession.Dismiss method 
            // will result in rollback of any text change.
            CompletionSession.SelectedCompletionSet.SelectionStatus =
                new CompletionSelectionStatus
                (
                    TheFirstCompletion,
                    true,
                    true
                );

            // dismiss the session
            CompletionSession.Dismiss();
        }
    }
}
