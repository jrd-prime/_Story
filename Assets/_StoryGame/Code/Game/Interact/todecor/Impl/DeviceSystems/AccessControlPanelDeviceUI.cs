using _StoryGame.Core.Providers.Localization;
using _StoryGame.Game.Extensions;
using _StoryGame.Game.Interact.todecor.Decorators.Active;
using _StoryGame.Game.UI.Impls.Viewer.Layers.Floating;
using _StoryGame.Infrastructure.Interact;
using UnityEngine;
using UnityEngine.UIElements;

namespace _StoryGame.Game.Interact.todecor.Impl.DeviceSystems
{
    public sealed class AccessControlPanelDeviceUI : ADeviceUI
    {
        [SerializeField] private string mainLabKey;


        private Button _codeInsertBtn;
        private Button _codeCloseBtn;
        private Button _mainOpenGateBtn;
        private Button _mainCloseBtn;
        private Label _mainTitleLab;

        protected override void OnAwake()
        {
            if (string.IsNullOrEmpty(mainLabKey))
                Debug.LogError($"{nameof(mainLabKey)} undefined. {name}");
        }

        protected override void InitializeElements()
        {
            // main panel
            _mainTitleLab = MainContainer.GetVElement<Label>("main-panel-title-lab", MainContainer.name);

            // btns
            _codeInsertBtn = MainContainer.GetVElement<Button>("code-req-insert-btn", MainContainer.name);
            _codeCloseBtn = MainContainer.GetVElement<Button>("code-req-close-btn", MainContainer.name);
            _mainOpenGateBtn = MainContainer.GetVElement<Button>("main-panel-open-gate-btn", MainContainer.name);
            _mainCloseBtn = MainContainer.GetVElement<Button>("main-panel-close-btn", MainContainer.name);
        }

        protected override void LocalizeElements()
        {
            _mainTitleLab.text = Localize(mainLabKey, ETable.SmallPhrase, ETextTransform.Upper);
        }

        protected override void OnShowPanel<T>(DevDa<T> da)
        {
            // var titleLab = MainContainer.GetVElement<Label>("title-label", MainContainer.name);
            // var descLab = MainContainer.GetVElement<Label>("desc-label", MainContainer.name);


            // titleLab.text = msg.LocalizedName;
            // descLab.text = msg.Question;

            var mainCloseBtnHandler = new ClickCompletionHandler<T>(
                _mainCloseBtn,
                (T)(object)EAccessControlPanelDialogResult.Close,
                da.CompletionSource
            );
            var codeCloseBtnHandler = new ClickCompletionHandler<T>(
                _codeCloseBtn,
                (T)(object)EAccessControlPanelDialogResult.Close,
                da.CompletionSource
            );

            var mainOpenGateBtnHandler = new ClickCompletionHandler<T>(
                _mainOpenGateBtn,
                (T)(object)EAccessControlPanelDialogResult.OpenGate,
                da.CompletionSource
            );

            var codeInsertBtnHandler = new ClickCompletionHandler<T>(
                _codeInsertBtn,
                (T)(object)EAccessControlPanelDialogResult.InsertFlash,
                da.CompletionSource
            );

            mainCloseBtnHandler.Register();
            codeCloseBtnHandler.Register();
            mainOpenGateBtnHandler.Register();
            codeInsertBtnHandler.Register();
        }

    }
}
