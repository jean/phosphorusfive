/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System;
using System.Linq;
using System.Security;
using System.Collections.Generic;

/// <summary>
///     Main namespace for Phosphorus core functionality.
/// </summary>
namespace p5.core
{
    /// <summary>
    ///     Application context, which your Active Events are raised through.
    /// </summary>
    public class ApplicationContext
    {
        /// <summary>
        ///     Class used as ticket when raising Active Events
        /// </summary>
        [Serializable]
        public class ContextTicket
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="p5.core.ApplicationContext+ContextTicket"/> class
            /// </summary>
            /// <param name="username">Username</param>
            /// <param name="role">Role</param>
            public ContextTicket(string username, string role, bool isDefault)
            {
                Username = username;
                Role = role;
                IsDefault = isDefault;
            }

            /// <summary>
            ///     Gets the username
            /// </summary>
            /// <value>The username</value>
            public string Username {
                get;
                private set;
            }

            /// <summary>
            ///     Gets the role
            /// </summary>
            /// <value>The password</value>
            public string Role {
                get;
                private set;
            }

            /// <summary>
            ///     Gets whether or not this is the "default Context user"
            /// </summary>
            /// <value>Whethere or not user is "default user"</value>
            public bool IsDefault {
                get;
                private set;
            }
        }

        private readonly ActiveEvents _registeredActiveEvents = new ActiveEvents();
        private readonly Loader.ActiveEventTypes _typesInstanceActiveEvents;
        private ContextTicket _ticket;
        private Node _whitelist;

        internal ApplicationContext (
            Loader.ActiveEventTypes instanceEvents, 
            Loader.ActiveEventTypes staticEvents, 
            ContextTicket ticket)
        {
            _ticket = ticket;
            _typesInstanceActiveEvents = instanceEvents;
            InitializeApplicationContext (staticEvents);
        }

        /// <summary>
        ///     Gets the Context user ticket
        /// </summary>
        /// <value>The ticket</value>
        public ContextTicket Ticket
        {
            get { return _ticket; }
        }

        /// <summary>
        ///     Gets or sets the whitelist of legal Active Events for the current context.
        /// </summary>
        public Node Whitelist {
            get { return _whitelist; }
            set { _whitelist = value; }
        }

        /// <summary>
        ///     Returns all Active Events registered within the current ApplicationContext object.
        /// </summary>
        /// <value>The active events</value>
        public IEnumerable<string> ActiveEvents
        {
            get { return _registeredActiveEvents.GetEvents ().Select (idx => idx.Name); }
        }

        /// <summary>
        ///     Changes the ticket for the context
        /// </summary>
        /// <param name="ticket">New ticket</param>
        public void UpdateTicket (ContextTicket ticket)
        {
            _ticket = ticket;
        }

        /// <summary>
        ///     Determines whether this instance has the event with the specified name.
        /// </summary>
        /// <returns><c>true</c> if this instance has event with the specified name; otherwise, <c>false</c></returns>
        /// <param name="name">Name</param>
        public bool HasEvent (string name)
        {
            return _registeredActiveEvents.HasEvent(name);
        }

        /// <summary>
        ///     Registers an instance Active Event listening object.
        /// </summary>
        /// <param name="instance">object to register for Active Event handling</param>
        public void RegisterListeningObject (object instance)
        {
            if (instance == null)
                throw new ArgumentNullException ("instance");

            // Recursively iterating the Type of the object given, until we reach an object where we know there won't exist
            // any Active Events
            var idxType = instance.GetType ();
            while (!idxType.FullName.StartsWith ("System.")) {

                // Checking to see if this type is registered in our list of types that has Active Events
                if (_typesInstanceActiveEvents.Types.ContainsKey (idxType)) {

                    // Retrieving the list of ActiveEvent/MethodInfo  for the currently iterated Type
                    foreach (var idxActiveEvent in _typesInstanceActiveEvents.Types [idxType].Events) {

                        // Adding Active Event
                        _registeredActiveEvents.AddMethod (
                            idxActiveEvent.Attribute.Name,
                            idxActiveEvent.Method,
                            instance);
                    }
                }

                // Continue iteration over Types until we reach a type we know does not have Active Events
                idxType = idxType.BaseType;
            }
        }

        /// <summary>
        ///     Unregisters an instance listening object.
        /// </summary>
        /// <param name="instance">object to unregister</param>
        public void UnregisterListeningObject (object instance)
        {
            if (instance == null)
                throw new ArgumentNullException ("instance");

            // Iterating over the Type until we find a type we know for a fact won't contains Active Events
            var type = instance.GetType ();
            while (!type.FullName.StartsWith ("System.")) {

                // Checking to see if our list of instance Active Events contains the currently iterated Type
                _registeredActiveEvents.RemoveMethod (instance);

                // finding base type, to continue iteration over its Type until we find a Type we're sure of does not 
                // contain Active Event declarations
                type = type.BaseType;
            }
        }

        /// <summary>
        ///     Raises one Active Event from lambda object
        /// </summary>
        /// <param name="name">name of Active Event to raise</param>
        /// <param name="pars">arguments to pass into the Active Event</param>
        public Node Raise (string name, Node pars = null)
        {
            if (Whitelist != null && !name.StartsWith (".") && !name.StartsWith ("_")) {
                return ExecuteSingleEventWithWhitelist (name, pars);
            } else {
                return _registeredActiveEvents.Raise (name, pars, this, _ticket);
            }
        }

        /*
         * Executes a single event according to whitelist definition.
         */
        private Node ExecuteSingleEventWithWhitelist (string name, Node pars)
        {
            // Whitelist provided, making sure event is legal.
            var definition = Whitelist[name];
            if (definition == null) {

                // Active Event invocation did not exist in Whitelist definition.
                throw new SecurityException (string.Format ("Caller tried to invoke illegal Active Event [{0}] according to whitelist definition", name));
            }

            // Checking if there exists one or more [pre-condition] objects with our definition.
            if (definition["pre-condition"] != null) {

                // Looping through all [post-conditions] for Active Event.
                foreach (var idxCondition in definition.Children.Where (ix => ix.Name == "pre-condition")) {

                    // Raising [post-condition] Active Event, which will throw if condition is not met.
                    var args = new Node ();
                    args.Add ("pre-condition", idxCondition);
                    args.Add ("lambda", pars);
                    Raise (".p5.lambda.whitelist.pre-condition." + idxCondition.Get<string> (this), args);
                }
            }

            // Raising the given Active Event, using the existing whitelist.
            var retVal = _registeredActiveEvents.Raise (name, pars, this, _ticket);

            // Checking if there exists one or more [post-condition] objects with our definition.
            if (definition["post-condition"] != null) {

                // Looping through all [post-conditions] for Active Event.
                foreach (var idxCondition in definition.Children.Where (ix => ix.Name == "post-condition")) {

                    // Raising [post-condition] Active Event, which will throw if condition is not met.
                    var args = new Node ();
                    args.Add ("post-condition", idxCondition);
                    args.Add ("lambda", pars);
                    Raise (".p5.lambda.whitelist.post-condition." + idxCondition.Get<string> (this), args);
                }
            }
            return retVal;
        }

        /*
         * initializes app context
         */
        private void InitializeApplicationContext (Loader.ActiveEventTypes staticEvents)
        {
            // Looping through each Type in static Active Events given
            foreach (var idxType in staticEvents.Types.Keys) {

                // Looping through each ActiveEvent/MethodInfo tuple in Type
                foreach (var idxAVTypeEvent in staticEvents.Types [idxType].Events) {

                    // Registering Active Event
                    _registeredActiveEvents.AddMethod (
                        idxAVTypeEvent.Attribute.Name, 
                        idxAVTypeEvent.Method, 
                        null);
                }
            }

            // Raising "initialize" Application Context Active Event
            Raise(".p5.core.initialize-application-context");
        }
    }
}
