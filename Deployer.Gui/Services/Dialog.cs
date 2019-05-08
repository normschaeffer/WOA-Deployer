﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deployer.Gui.Views;
using MahApps.Metro.Controls.Dialogs;

namespace Deployer.Gui.Services
{
    public class Dialog : IDialog, IPrompt
    {
        private readonly IDialogCoordinator coordinator;
        private readonly ViewMappings viewMappings;

        public Dialog(IDialogCoordinator coordinator, ViewMappings viewMappings)
        {
            this.coordinator = coordinator;
            this.viewMappings = viewMappings;
        }

        public Task ShowAlert(object owner, string title, string text)
        {
            return coordinator.ShowMessageAsync(owner, title, text);
        }

        public async Task<DialogResult> ShowConfirmation(object owner, string title, string text)
        {
            var result = await coordinator.ShowMessageAsync(owner, title, text, MessageDialogStyle.AffirmativeAndNegative);
            return result == MessageDialogResult.Affirmative ? DialogResult.Yes : DialogResult.No;
        }

        public async Task<Option> PickOptions(string markdown, IEnumerable<Option> options)
        {
            var markdownViewerWindow = new MarkdownViewerWindow();
            Option option;
            using (var viewModel = new AutoMessageViewModel(markdown, options, markdownViewerWindow))
            {
                markdownViewerWindow.DataContext = viewModel;

                var wnd = markdownViewerWindow;
                await wnd.ShowDialogAsync();
                option = viewModel.SelectedOption;
            }

            return option;
        }

        public async Task<DialogResult> Show(string key, object context)
        {
            var dialogWindow = new DialogWindow();
            var options = new List<Option>
            {
                new Option("OK", OptionValue.OK),
                new Option("Cancel", OptionValue.Cancel)
            };

            var dialogViewModel = new DialogViewModel("Deployment options", context, options, dialogWindow);
            dialogWindow.DataContext = dialogViewModel;

            var confirmed = (await dialogWindow.ShowDialogAsync()).HasValue && dialogViewModel.SelectedOption?.OptionValue == OptionValue.OK;

            return confirmed ? DialogResult.Yes : DialogResult.No;
        }
    }
}