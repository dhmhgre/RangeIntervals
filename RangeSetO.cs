﻿/*
 GNU AFFERO GENERAL PUBLIC LICENSE
    Version 3, 19 November 2007
Copyright(c) [2017] [Grigoris Dimitroulakos] 
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RangeIntervals {

    

    /// <summary>
    /// This class traverses the successive elements of a range of elements
    /// with the given step.
    /// </summary>
    /// <typeparam name="T">Type of range element</typeparam>
    /// <seealso cref="System.Collections.Generic.IEnumerator{T}" />
    public class RangeIterator<T> : IEnumerator<T> where T :  IComparable<T> {
        private Range<T> m_range;
        private T m_currentElement;

        public RangeIterator(Range<T> range) {
            m_range = range;
            Reset();
        }
        public T Current { get { return m_currentElement; } }


        public void Dispose() {
            /*Nothing to do*/
        }

        public bool MoveNext() {
            m_currentElement = m_range.Next(m_currentElement);
            if (m_currentElement.CompareTo(m_range.Max)>0 ) {
                return false;
            }
            return true;
        }

        public void Reset() {
            m_currentElement = m_range.Prev(m_range.Min);
        }

        object IEnumerator.Current {
            get { return Current; }
        }
    }

    public enum RangeProximityToElement {
        RP_NONE,
        RP_LEFT,
        RP_RIGHT,
        RP_IN
    }

    /// <summary>
    /// Represents a range interval as an immutable object. After the  object
    /// is created it cannot be changed. Any change in the min max fields results
    /// in a new object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Range<T> : IEnumerable<T> where T : IComparable<T> {
        private T m_min = default(T);
        private T m_max = default(T);

        public Range() {

        }

        public IEnumerator<T> GetEnumerator() {
            return new RangeIterator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public Range(T min, T max) {
            m_min = min;
            m_max = max;
            if (max.CompareTo(min) < 0) {
                throw new ArgumentOutOfRangeException();
            }
        }
        /// <summary>
        /// Returns true if the element is in range
        /// </summary>
        /// <param name="elem">The elem.</param>
        /// <returns></returns>
        public bool IsInRange(T elem) {
            if (elem.CompareTo(m_max) <= 0 && elem.CompareTo(m_min) >= 0) {
                return true;
            }
            return false;
        }

        public RangeProximityToElement IsNeighbor(T elem) {

            if (elem.CompareTo(Prev(m_min)) == 0) {
                return RangeProximityToElement.RP_LEFT;
            }
            else if (elem.CompareTo(Next(m_max)) == 0){
                return RangeProximityToElement.RP_RIGHT;
            }
            else if ( IsInRange(elem) ) {
                return RangeProximityToElement.RP_IN;
            }
            else {
                return RangeProximityToElement.RP_NONE;
            }
        }

        /// <summary>
        /// Returns the next element of x  ( Mandatory for iterator )
        /// </summary>
        /// <param name="x">An element in the T space</param>
        /// <returns></returns>
        public virtual T Next(T x) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the previous element of x  ( Mandatory for iterator )
        /// </summary>
        /// <param name="x">An element in the T space</param>
        /// <returns></returns>
        public virtual T Prev(T x) {
            throw new NotImplementedException();
        }


        public override string ToString() {
            return "Min : " + m_min.ToString() + " -  Max: " + m_max.ToString();
        }

        public virtual T Min {
            get { return m_min; }
            set { m_min = value; }
        }

        public virtual T Max {
            get { return m_max; }
            set { m_max = value; }
        }
    }

    /// <summary>
    /// This class defines an iterator for the range set collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RangeSetIterator<T, Y> : IEnumerator<T> where T : Range<Y>, new() where Y : IComparable<Y> {
        private List<T> m_collection;
        private int m_currentIndex;
        private T m_currentItem;
        public T Current { get { return m_currentItem; } }
        private bool m_inReverse;

        public RangeSetIterator(List<T> collection, bool inReverse = false) {
            m_collection = collection;
            m_inReverse = inReverse;
            Reset();
        }

        public bool MoveNext() {
            if (!m_inReverse) {
                if (++m_currentIndex >= m_collection.Count) {
                    return false;
                }
                m_currentItem = m_collection[m_currentIndex];
                return true;
            } else {
                if (--m_currentIndex < 0) {
                    return false;
                }
                m_currentItem = m_collection[m_currentIndex];
                return true;
            }
        }

        object IEnumerator.Current {
            get { return m_currentItem; }
        }

        public void Reset() {
            if (m_inReverse) {
                m_currentIndex = m_collection.Count;
            } else {
                m_currentIndex = -1;
            }

        }

        public void Dispose() {

        }
    }


    /// <summary>
    /// The algorithm of RangeSet separates the whole space in alternate
    /// EMPTY and NON-EMPTY intervals. These intervals are indexed with even and
    /// odd indices respectivelly. The algorithm to add a new range specifies
    /// in which intervals the range's minimum and maximum points lie. Then it
    /// determines how many existing ranges overlaps to delete them before
    /// inserting the new range
    /// </summary>
    /// <typeparam name="T">The type of interval</typeparam>
    /// <typeparam name="Y">The type of range elements</typeparam>
    /// <seealso cref="System.Collections.Generic.IEnumerable{T}" />
    public class RangeSetO<T, Y> : IEnumerable<T> where T : Range<Y>, new() where Y : IComparable<Y> {
        private List<T> m_rangeList = new List<T>();

        private bool m_discreteInterval;

        public RangeSetO(bool discreteInterval) {
            m_discreteInterval = discreteInterval;
        }

        public bool M_DiscreteInterval {
            get => m_discreteInterval;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void AddSet(RangeSetO<T, Y> set) {
            foreach (T range in set) {
                AddRange(range);
            }
        }

        public bool IsInSet(Y elem) {
            foreach (var e in this) {
                if (e.IsInRange(elem)) {
                    return true;
                }
            }
            return false;
        }

        public void AddRange(Y unit) {
            int count=1;
            RangeProximityToElement prox;
            if (m_discreteInterval) {
                while (count != 0 && m_rangeList.Count != 0) {
                    foreach (T range in m_rangeList) {
                        prox = range.IsNeighbor(unit);
                        switch (prox) {
                            case RangeProximityToElement.RP_LEFT:
                                AddRange(new T() {Min = unit, Max = range.Min});
                                count = 1;
                                goto outfor;
                                break;
                            case RangeProximityToElement.RP_RIGHT:
                                AddRange(new T() {Min = range.Max, Max = unit});
                                count = 1;
                                goto outfor;
                                break;
                            case RangeProximityToElement.RP_IN:
                                return;
                            default:
                                count = 0;
                                break;
                        }
                    }
                outfor: ;
                }
                AddRange(new T() { Min = unit, Max = unit });
            }
            else {
                AddRange(new T(){Min = unit, Max = unit});
            }


        }

        public void AddRange(T rng) {

            T newRange;
            Y minPoint, maxPoint;

            // Find the region in the range intervals where
            // the minimum point of rng lies. Produce the index
            // from where on the indices of the range list change


            int indexS = 0, indexF = 0;  // The two variables represent the index of the region
                                         // where the Min and Max point of the given rng lies
                                         // respectively

            int S, F;                     // The two variables represent the first and the last
                                          // index of the ranges in the existing RangeSet collection
                                          // overlaped by the given rng
            int r = 0;                   // iterator variable
            foreach (var range in m_rangeList) {
                // The loop breaks whichever of the following ifs
                // conditions results to true. Traversal is performed
                // from left to write checking first if the point (rng.Min)
                // lies in the current's range Min region and subsequently
                // to the range's Max region
                // Regions are numbered according to aforementioned discipline
                if (rng.Min.CompareTo(range.Min) < 0) {
                    indexS = r;
                    break;
                }

                if (rng.Min.CompareTo(range.Max) <= 0) {
                    indexS = r + 1;
                    break;
                }

                if (range == m_rangeList.Last()) {
                    indexS = r + 2;
                }
                r = r + 2; // Every range in the existing collection covers to regions:
                           // 1) an empty region before the range having an odd index and
                           // 2) a subsequent non-empty region represented by the range itself having
                           // even index. That's why the step from range to range is 2
            }
            r = 0;
            foreach (var range in m_rangeList) {
                // The loop breaks whichever of the following ifs
                // conditions results to true. Traversal is performed
                // from left to write checking first if the point (rng.Max)
                // lies in the current's range Min region and subsequently
                // to the range's Max region
                // Regions are number according to aforementioned discipline
                if (rng.Max.CompareTo(range.Min) < 0) {
                    indexF = r;
                    break;
                }

                if (rng.Max.CompareTo(range.Max) <= 0) {
                    indexF = r + 1;
                    break;
                }

                if (range == m_rangeList.Last()) {
                    indexF = r + 2;
                }
                r = r + 2; // Every range in the existing collection covers to regions:
                           // 1) an empty region before the range having an odd index and
                           // 2) a subsequent non-empty region represented by the range itself having
                           // even index. That's why the step from range to range is 2
            }

            S = indexS / 2;     // The index of the first range crossed by the given range
            F = (indexF - 1) / 2; // The index of the final range crossed by the given range

            if (indexS == indexF) {
                if (indexS % 2 == 0) {

                    // Even indexS represents a empty space. The given range lies completely
                    // on empty space.
                    // Create the new range
                    newRange = new T() { Max = rng.Max, Min = rng.Min };

                    // Add the newrange in the appropriate position
                    m_rangeList.Insert(S, newRange);
                } else {
                    // The given range lies completely on and existing range.
                    // Do nothing as there is an overlap
                }
            } else {
                // At least one existing interval is crossed by the given range.
                // A new range will have to be created and the existing overlaped
                // intervals must be removed from the collection

                // Check whether the minimum of the given range lies on EMPTY or
                // NON-EMPTY space
                if (indexS % 2 == 0) {
                    // rng minimum is the minimum of the new range interval
                    minPoint = rng.Min;
                } else {
                    // The overlaped's range minimum is the minimum of the new interval
                    minPoint = m_rangeList[S].Min;
                }

                // Check whether the maximum of the given range lies on EMPTY or
                // NON-EMPTY space
                if (indexF % 2 == 0) {
                    // rng maximum is on an empty space
                    // rng maximum is the maximum of the new range interval
                    maxPoint = rng.Max;
                } else {
                    // The overlaped's range maximum is the maximum of the new interval
                    maxPoint = m_rangeList[F].Max;
                }

                // Even indexS represents a empty space
                // Create the new range
                newRange = new T() { Min = minPoint, Max = maxPoint };

                // Delete intervals crossing with the new interval
                for (int i = S; i <= F; i++) {
                    m_rangeList.RemoveAt(S);
                }

                // Add the newrange in the appropriate position
                m_rangeList.Insert(S, newRange);
            }
        }

        public IEnumerator<T> GetEnumerator() {
            return new RangeSetIterator<T, Y>(m_rangeList);
        }

        public override string ToString() {
            StringBuilder s = new StringBuilder();
            foreach (T range in m_rangeList) {
                s.Append(range.ToString());
                s.AppendLine();
            }
            return s.ToString();
        }

    }
}
