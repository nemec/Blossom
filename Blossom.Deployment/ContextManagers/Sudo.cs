﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment.ContextManagers
{
    public class Sudo : ContextManager
    {
        private string _administratorPassword;

        private bool _previousValue;

        public Sudo(IDeploymentContext context, string administratorPassword)
            : base(context)
        {
            throw new NotImplementedException(); // TODO Figure out how to give password to sudo
            _administratorPassword = administratorPassword;
            base.Begin();
        }

        protected override void Enter()
        {
            _previousValue = Context.Environment.Remote.IsElevated;
            Context.Environment.Remote.IsElevated = true;
        }

        protected override void Exit()
        {
            Context.Environment.Remote.IsElevated = _previousValue;
        }
    }
}