﻿//Copyright 2004 - $Date: 2008-11-15 23:58:07 +0100 (za, 15 nov 2008) $ by PeopleWare n.v..

//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at

//http://www.apache.org/licenses/LICENSE-2.0

//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

#region Using

using NUnit.Framework;

using PPWCode.Util.OddsAndEnds.I.Extensions;

#endregion

namespace PPWCode.Util.OddsAndEnds.Test_I
{
    /// <summary>
    /// This is a test class for StringExtensionsTest and is intended
    /// to contain all StringExtensionsTest Unit Tests
    /// </summary>
    [TestFixture]
    public class StringExtensionsTest
    {
        #region IsCaptialized

        [Test, Description("IsCapitalized OneLowercase")]
        public void IsCapitalizedTestOneLowercaseChar()
        {
            Assert.IsFalse("a".StartWithACapital());
        }

        [Test, Description("IsCapitalized OneUppercase")]
        public void IsCapitalizedTestOneUppercaseChar()
        {
            Assert.IsTrue("A".StartWithACapital());
        }

        [Test, Description("IsCapitalized MultipleLowercase")]
        public void IsCapitalizedTestMultipleLowercaseCharacters()
        {
            Assert.IsFalse("andras pandy".StartWithACapital());
        }

        [Test, Description("IsCapitalized MultipleUppercase")]
        public void IsCapitalizedTestMultipleUppercaseCharacters()
        {
            Assert.IsTrue("AGNES PANDY".StartWithACapital());
        }

        [Test, Description("IsCapitalized Sentence")]
        public void IsCapitalizedTestSentce()
        {
            Assert.IsTrue("The dissolved pandies recorded their latest album in 2001".StartWithACapital());
        }

        #endregion
    }
}
