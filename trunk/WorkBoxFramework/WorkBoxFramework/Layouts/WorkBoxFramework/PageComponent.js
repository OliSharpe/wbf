// Copyright (c) Islington Council 2010-2013
// Author: Oli Sharpe  (oli@gometa.co.uk)
//
// This file is part of the Work Box Framework.
//
// The Work Box Framework is free software: you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public License as  
// published by the Free Software Foundation, either version 2.1 of the 
// License, or (at your option) any later version.
//
// The Work Box Framework is distributed in the hope that it will be 
// useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with Foobar.  If not, see <http://www.gnu.org/licenses/>.

// Inspired by blogs:
// http://www.sharepointnutsandbolts.com/2010/02/ribbon-customizations-dropdown-controls.html
// http://patrickboom.wordpress.com/2010/05/25/adding-a-custom-company-menu-tab-with-dynamic-menu-on-the-ribbon/


Type.registerNamespace('WorkBoxFramework');


// RibbonApp Page Component
WorkBoxFramework.PageComponent = function () {
    WorkBoxFramework.PageComponent.initializeBase(this);
}


WorkBoxFramework.PageComponent.initialize = function () {
    ExecuteOrDelayUntilScriptLoaded(Function.createDelegate(null, WorkBoxFramework.PageComponent.initializePageComponent), 'SP.Ribbon.js');
}


WorkBoxFramework.PageComponent.initializePageComponent = function () {
    var ribbonPageManager = SP.Ribbon.PageManager.get_instance();
    if (null !== ribbonPageManager) {
        ribbonPageManager.addPageComponent(WorkBoxFramework.PageComponent.instance);
        ribbonPageManager.get_focusManager().requestFocusForComponent(WorkBoxFramework.PageComponent.instance);
    }
}


WorkBoxFramework.PageComponent.refreshRibbonStatus = function () {
  SP.Ribbon.PageManager.get_instance().get_commandDispatcher().executeCommand(Commands.CommandIds.ApplicationStateChanged, null);
}


WorkBoxFramework.PageComponent.prototype = {
  getFocusedCommands: function () {
      return [];
  },
  getGlobalCommands: function () {
      return getGlobalCommands();
  },
  isFocusable: function () {
      return true;
  },
  receiveFocus: function () {
      return true;
  },
  yieldFocus: function () {
      return true;
  },
  canHandleCommand: function (commandId) {
      return commandEnabled(commandId);
  },
  handleCommand: function (commandId, properties, sequence) {
      return handleCommand(commandId, properties, sequence);
  }
}


// Register classes
WorkBoxFramework.PageComponent.registerClass('WorkBoxFramework.PageComponent', CUI.Page.PageComponent);
WorkBoxFramework.PageComponent.instance = new WorkBoxFramework.PageComponent();


// Notify waiting jobs
NotifyScriptLoadedAndExecuteWaitingJobs('/_layouts/WorkBoxFramework/PageComponent.js');

