﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposableReactiveList.cs" company="RHEA System S.A.">
//    Copyright (c) 2020-2020 RHEA System S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski.
// 
//    This file is part of DEHP Common Library
// 
//    The DEHPCommon is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The DEHPCommon is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
// 
//    You should have received a copy of the GNU Lesser General Public License
//    along with this program; if not, write to the Free Software Foundation,
//    Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.Mvvm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ReactiveUI;

    /// <summary>
    /// Special type of <see cref="ReactiveList{T}"/> that is capable of disposing of child items when items are removed from the list
    /// Normal "remove" methods are overwritten and made Obsolete, so a developer is forced to make a choice between "remove and dispose", or "remove without dispose".
    /// </summary>
    public class DisposableReactiveList<T> : ReactiveList<T> where T : IDisposable
    {
        /// <summary>
        /// Overrides the method implementation in <see cref="ReactiveList{T}"/> and adds an <see cref="ObsoleteAttribute"/>
        /// so the code doesn't compile anymore when the method is unexpectedly being used.
        /// <a href="https://github.com/RHEAGROUP/CDP4-IME-Community-Edition/wiki/MVVM#disposablereactivelistt">For more info</a>
        /// </summary>
        [Obsolete("Clear is deprecated, please use Clear(bool dispose) instead.", true)]
        public new void Clear()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Wrapper for executing <see cref="Clear(bool)"/> method WITH automatic disposal of concerning objects of type <typeparamref name="T" />
        /// </summary>
        public void ClearAndDispose()
        {
            this.Clear(true);
        }

        /// <summary>
        /// Wrapper for executing <see cref="Clear(bool)"/> method WITHOUT automatic disposal of concerning objects of type <typeparamref name="T" />
        /// </summary>
        public void ClearWithoutDispose()
        {
            this.Clear(false);
        }

        /// <summary>
        /// Clear all items of type <typeparamref name="T" /> from this instance
        /// </summary>
        /// <param name="dispose">States if the removed objects of type <typeparamref name="T" /> should also be disposed on removal</param>
        private void Clear(bool dispose)
        {
            if (dispose)
            {
                foreach (var item in this)
                {
                    this.TryDispose(item);
                }
            }

            base.Clear();
        }

        /// <summary>
        /// Overrides the method implementation in <see cref="ReactiveList{T}"/> and adds an <see cref="ObsoleteAttribute"/>
        /// so the code doesn't compile anymore when the method is unexpectedly being used.
        /// </summary>
        [Obsolete("RemoveAll(IEnumerable<T> items) is deprecated, please use RemoveAll(IEnumerable<T> items, bool dispose) instead.", true)]
        public new void RemoveAll(IEnumerable<T> items)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Wrapper for executing <see cref="RemoveAll(IEnumerable{T}, bool)"/> method WITH automatic disposal of concerning objects of type <typeparamref name="T" />
        /// </summary>
        public void RemoveAllAndDispose(IEnumerable<T> items)
        {
            this.RemoveAll(items, true);
        }

        /// <summary>
        /// Wrapper for executing <see cref="RemoveAll(IEnumerable{T}, bool)"/> method WITHOUT automatic disposal of concerning objects of type <typeparamref name="T" />
        /// </summary>
        public void RemoveAllWithoutDispose(IEnumerable<T> items)
        {
            this.RemoveAll(items, false);
        }

        /// <summary>
        /// Remove the specific items in <paramref name="items"/> from this instance
        /// </summary>
        /// <param name="items">The objects of type <typeparamref name="T" /> that need to be removed from this instance</param>
        /// <param name="dispose">States if the removed objects of type <typeparamref name="T" /> should also be disposed on removal</param>
        private void RemoveAll(IEnumerable<T> items, bool dispose)
        {
            var disposables = items as T[] ?? items.ToArray();

            if (dispose)
            {
                foreach (var item in disposables)
                {
                    this.TryDispose(item);
                }
            }

            base.RemoveAll(disposables);
        }

        /// <summary>
        /// Overrides the method implementation in <see cref="ReactiveList{T}"/> and adds an <see cref="ObsoleteAttribute"/>
        /// so the code doesn't compile anymore when the method is unexpectedly being used.
        /// </summary>
        [Obsolete("RemoveAt(int index) is deprecated, please use RemoveAt(int index, bool dispose) instead.", true)]
        public new void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Wrapper for executing <see cref="RemoveAt(int, bool)"/> method WITH automatic disposal of concerning objects of type <typeparamref name="T" />
        /// </summary>
        public void RemoveAtAndDispose(int index)
        {
            this.RemoveAt(index, true);
        }

        /// <summary>
        /// Wrapper for executing <see cref="RemoveAt(int, bool)"/> method WITHOUT automatic disposal of concerning objects of type <typeparamref name="T" />
        /// </summary>
        public void RemoveAtWithoutDispose(int index)
        {
            this.RemoveAt(index, false);
        }

        /// <summary>
        /// Remove the item at specific position <paramref name="index"/> from this instance
        /// </summary>
        /// <param name="index">Index of the object of type <typeparamref name="T" /> that needs to be removed from this instance</param>
        /// <param name="dispose">States if the removed objects of type <typeparamref name="T" /> should also be disposed on removal</param>
        private void RemoveAt(int index, bool dispose)
        {
            try
            {
                if (dispose)
                {
                    var item = this[index];
                    this.TryDispose(item);
                }
            }
            catch (Exception)
            {
                // index probably not found
                // Perform base class functionality anyway
            }
            finally
            {
                base.RemoveAt(index);
            }
        }

        /// <summary>
        /// Overrides the method implementation in <see cref="ReactiveList{T}"/> and adds an <see cref="ObsoleteAttribute"/>
        /// so the code doesn't compile anymore when the method is unexpectedly being used.
        /// </summary>
        [Obsolete("Remove(T item) is deprecated, please use  Remove(T item, bool dispose) instead.", true)]
        public new bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Wrapper for executing <see cref="Remove(T, bool)"/> method WITH automatic disposal of concerning objects of type <typeparamref name="T" />
        /// </summary>
        public bool RemoveAndDispose(T item)
        {
            return this.Remove(item, true);
        }

        /// <summary>
        /// Wrapper for executing <see cref="Remove(T, bool)"/> method WITHOUT automatic disposal of concerning objects of type <typeparamref name="T" />
        /// </summary>
        public bool RemoveWithoutDispose(T item)
        {
            return this.Remove(item, false);
        }

        /// <summary>
        /// Removes the <paramref name="item"/> of type <typeparamref name="T" /> from this instance
        /// </summary>
        /// <param name="item">Item of type <typeparamref name="T" /> that needs to be removed from this instance</param>
        /// <param name="dispose">States if the removed objects of type <typeparamref name="T" /> should also be disposed on removal</param>
        /// <returns>true if removed from this instance, otherwise false</returns>
        private bool Remove(T item, bool dispose)
        {
            if (dispose)
            {
                this.TryDispose(item);
            }

            return base.Remove(item);
        }

        /// <summary>
        /// Overrides the method implementation in <see cref="ReactiveList{T}"/> and adds an <see cref="ObsoleteAttribute"/>
        /// so the code doesn't compile anymore when the method is unexpectedly being used.
        /// </summary>
        [Obsolete("RemoveRange(int index, int count) is deprecated, please use  RemoveRange(int index, int count, bool dispose) instead.", true)]
        public new void RemoveRange(int index, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Wrapper for executing <see cref="RemoveRange(int, int, bool)"/> method WITH automatic disposal of concerning objects of type <typeparamref name="T" />
        /// </summary>
        public void RemoveRangeAndDispose(int index, int count)
        {
            this.RemoveRange(index, count, true);
        }

        /// <summary>
        /// Wrapper for executing <see cref="RemoveRange(int, int, bool)"/> method WITHOUT automatic disposal of concerning objects of type <typeparamref name="T" />
        /// </summary>
        public void RemoveRangeWithoutDispose(int index, int count)
        {
            this.RemoveRange(index, count, false);
        }

        /// <summary>
        /// Removes <paramref name="count"/> consecutive items of type <typeparamref name="T" /> starting at position <paramref name="index"/> from this instance
        /// </summary>
        /// <param name="index">The starting position in the <see cref="IList{T}"/> of the items to be removed</param>
        /// <param name="count">Number of items to be removed consecutively, starting at position <paramref name="index"/></param>
        /// <param name="dispose">States if the removed objects of type <typeparamref name="T" /> should also be disposed on removal</param>
        private void RemoveRange(int index, int count, bool dispose)
        {
            if (dispose)
            {
                foreach (var itemIndex in Enumerable.Range(index, count))
                {
                    this.TryDispose(this[itemIndex]);
                }
            }

            base.RemoveRange(index, count);
        }

        /// <summary>
        /// Try to dispose an item of type <typeparamref name="T" /> in this instance
        /// </summary>
        /// <param name="item">The item to dispose</param>
        private void TryDispose(T item)
        {
            if (this.Contains(item))
            {
                item.Dispose();
            }
        }
    }
}
