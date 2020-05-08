using System;
using System.Collections.Generic;
using System.Text;

namespace GeoLib
{
    /// <summary>
    /// Class to help with reordering a point set to minimise the perimeter.
    /// </summary>
    public class CTravellingSalesman : LinkedList<C2DPoint>
    {
        /// <summary>
	    /// Constructor
        /// </summary>
	    public CTravellingSalesman() {}

        /// <summary>
	    /// Destructor
        /// </summary>
	    ~CTravellingSalesman() {}

        /// <summary>
	    /// Allocates points from a set by removing them from the set
        /// </summary>
        public void SetPointsDirect(List<C2DPoint> Points)
        {
            Clear();

            for (int i = 0; i < Points.Count; i++)
            {
                this.AddLast(Points[i]);

            }
            Points.Clear();
        }

        /// <summary>
	    /// Extracts the points into the set
        /// </summary>
        public void ExtractPoints(List<C2DPoint> Points)
        {
            LinkedListNode<C2DPoint> Iter = First;
            while ( Iter != null)
            {
                Points.Add(Iter.Value);
                Iter = Iter.Next;
            }

            this.Clear();
        }

        /// <summary>
	    /// Inserts a point optimally into this 
        /// </summary>
        public void InsertOptimally(C2DPoint pt)
        {
	        // Special cases if there are less than 3 points
	        if (Count < 2)
	        {
		        if (Count == 1)
			        this.AddLast(pt);
		        return;
	        }

	        // a pointer to a point and the point after it
	        C2DPoint ptH1;
	        C2DPoint ptH2;
	        // Set up an iterator and the 2 points.


            ptH1 = First.Value;
            ptH2 = First.Next.Value;

            LinkedListNode<C2DPoint> IterInsert = First.Next;
            LinkedListNode<C2DPoint> Iter = First.Next;
	        // Find the assumed minimum distance expansion. i.e. if we insert the point
	        // between the first and second points what is the increase in the route.
	        double dMinExp = ptH1.Distance(pt) + pt.Distance(ptH2) - ptH1.Distance(ptH2);

	        // Now go through all the other positions and do the same test, recording the
	        // optimal position.
            Iter = Iter.Next;
            
	        while (Iter != null)
	        {
		        ptH1 = ptH2;
		        ptH2 = Iter.Value;

		        double dExp = ptH1.Distance(pt) + pt.Distance(ptH2) - ptH1.Distance(ptH2);
		        if (dExp < dMinExp)
		        {
			        dMinExp = dExp;
			        IterInsert = Iter;
		        }
		        Iter = Iter.Next;
	        }
	        // Finally just insert it in the list at the best place.
            this.AddBefore(IterInsert, pt);

        }

        /// <summary>
	    /// Refines the set, trying to find optimal positions for the points
        /// </summary>
        public void Refine()
        {
	        // CHECK FOR LESS THAN 2 ITEMS.
	        if (Count < 2)
		        return;

	        bool bRepeat = true;

	        int nIt = 0;

	        while (bRepeat && nIt < conMaxIterations)
	        {
		        nIt++;
                LinkedListNode<C2DPoint> IterRemove = First;
		   //     std::list<C2DPoint*>::iterator IterRemove = m_Points->begin(); // The item considered for removal.
		        IterRemove = IterRemove.Next;
          //    IterRemove++; // We don't want to remove the first point
                LinkedListNode<C2DPoint> IterRemoveLimit = Last;
		   //     std::list<C2DPoint*>::iterator IterRemoveLimit = m_Points->end(); // The last item considered for removal.
		    //    IterRemoveLimit = IterRemoveLimit.Previous; // No Need
           //    IterRemoveLimit--; // We don't want to remove the end. (This IS the end but we won't go that far)

		        bRepeat = false;
		        while (IterRemove != IterRemoveLimit)
		        {

                    LinkedListNode<C2DPoint> IterInsert = IterRemove;
			        // Move back along the list untill we find a better place. Only go so far.
			     //   std::list<C2DPoint*>::iterator IterInsert = IterRemove; // The item considered for a new place.
                    IterInsert = IterInsert.Previous;
              //    IterInsert--;
			        int nCountBack = 1;
			        bool bFound = false;
                    LinkedListNode<C2DPoint> IterRemoveBefore;
                    LinkedListNode<C2DPoint> IterRemoveAfter;
                    LinkedListNode<C2DPoint> IterInsertBefore;
                    //std::list<C2DPoint*>::iterator IterRemoveBefore;
                    //std::list<C2DPoint*>::iterator IterRemoveAfter;
                    //std::list<C2DPoint*>::iterator IterInsertBefore;
                    while (nCountBack < conRefineProximity && IterInsert != First)
			        {
				        IterRemoveBefore = IterRemove;
                        IterRemoveBefore = IterRemoveBefore.Previous;
				  //      IterRemoveBefore--; // This is the point before the removal
				        IterRemoveAfter = IterRemove;
                        IterRemoveAfter = IterRemoveAfter.Next;
                        //IterRemoveAfter++; // This is the point after the removal.
				        IterInsertBefore = IterInsert;
                        IterInsertBefore = IterInsertBefore.Previous;
				   //     IterInsertBefore--;// This is the point before the potential insertion point.

	        //			assert((**IterRemove) != (**IterInsert));

				        double dCurrentPerimPart = IterRemoveBefore.Value.Distance(IterRemove.Value) + 
										        IterRemove.Value.Distance(IterRemoveAfter.Value) +
										        IterInsertBefore.Value.Distance(IterInsert.Value);
				        double dNewPerimPart = IterRemoveBefore.Value.Distance(IterRemoveAfter.Value) +
										        IterInsertBefore.Value.Distance(IterRemove.Value) +
										        IterRemove.Value.Distance(IterInsert.Value);
				        if (dNewPerimPart < dCurrentPerimPart)
				        {
					        C2DPoint ptRemove = IterRemove.Value;
                            this.Remove(IterRemove);
                            this.AddBefore(IterInsert, ptRemove);
					   //     m_Points->erase(IterRemove);
					    //    m_Points->insert( IterInsert, ptRemove );
					        bFound = true;
					        IterRemove = IterRemoveAfter;//IterInsert; // WE HAVE GONE BACK SO PUT THE REMOVAL POINT BACK HERE AND SEARCH AGAIN.
					        break;
				        }
                        IterInsert = IterInsert.Previous;
				        nCountBack++;
			        }
        			
			        // Now go forward along the list untill we find a better place. Only go so far.
			        int nCountForward = 2;
			        IterInsert = IterRemove; // The item considered for a new place.
                    IterInsert = IterInsert.Next;
			        if (IterInsert != null)
                        IterInsert = IterInsert.Next; // Go forward 2 to avoid taking it out and putting it back in the same place.
			        else
				        nCountForward = conRefineProximity; // get out

			        while (!bFound && nCountForward < conRefineProximity && IterInsert != null)
			        {
				        IterRemoveBefore = IterRemove;
                        IterRemoveBefore = IterRemoveBefore.Previous; // This is the point before the removal
				        IterRemoveAfter = IterRemove;
                        IterRemoveAfter = IterRemoveAfter.Next; // This is the point after the removal.
				        IterInsertBefore = IterInsert;
                        IterInsertBefore = IterInsertBefore.Previous;// This is the point before the potential insertion point.

        //				assert((**IterRemove) != (**IterInsert));

				        double dCurrentPerimPart = IterRemoveBefore.Value.Distance(IterRemove.Value) + 
										        IterRemove.Value.Distance(IterRemoveAfter.Value) +
										        IterInsertBefore.Value.Distance(IterInsert.Value);
				        double dNewPerimPart = IterRemoveBefore.Value.Distance(IterRemoveAfter.Value) +
										       IterInsertBefore.Value.Distance(IterRemove.Value) +
										        IterRemove.Value.Distance(IterInsert.Value);
				        if (dNewPerimPart  < dCurrentPerimPart)
				        {
					        C2DPoint ptRemove = IterRemove.Value;
                            this.Remove( IterRemove );
                            this.AddBefore(  IterInsert, ptRemove );
					   //     m_Points->erase(IterRemove);
					    //    m_Points->insert( IterInsert, ptRemove );
					        IterRemove = IterRemoveAfter; 
				        //	IterRemove ++;
					        bFound = true; 
					        break;
				        }
                        IterInsert = IterInsert.Next;
				      //  IterInsert++;
				        nCountForward++;
			        }
        			
			        if (bFound) bRepeat = true;

			        if (!bFound)
				        IterRemove = IterRemove.Next;
		        }	// while Remove.
	        } // while bRepeat.

        }

        /// <summary>
	    /// Refines the set, trying to find optimal positions for the points
        /// </summary>
        public void Refine2()
        {
	        int nSize = Count;
	        if (nSize < 4)
		        return;

            LinkedListNode<C2DPoint> Iter = First;
            LinkedListNode<C2DPoint> Iter1;
            LinkedListNode<C2DPoint> Iter2;
            LinkedListNode<C2DPoint> Iter3;

	  //      std::list<C2DPoint*>::iterator Iter = m_Points->begin(); 
	    //    std::list<C2DPoint*>::iterator Iter1;
	    //    std::list<C2DPoint*>::iterator Iter2;
	    //    std::list<C2DPoint*>::iterator Iter3;
	        int nIndex = 0;
	        int nIndexLimit = nSize - 3;

	        bool bRepeat = true;

	        int nIt = 0;

	        while (bRepeat && nIt < conMaxIterations)
	        {
		        nIt++;
		        bRepeat = false;
		        while (nIndex < nIndexLimit)
		        {
			        Iter1 = Iter;
                    Iter1 = Iter1.Next;
			        Iter2 = Iter;	
			        Iter2 = Iter2.Next;
                    Iter2 = Iter2.Next;
			        Iter3 = Iter;
                    Iter3 = Iter3.Next;
			        Iter3 = Iter3.Next;
			        Iter3 = Iter3.Next;

			        double dCurrentPerimPart = Iter.Value.Distance(Iter1.Value) + 
										       Iter1.Value.Distance(Iter2.Value) +
										       Iter2.Value.Distance(Iter3.Value);

			        double dNewPerimPart = Iter.Value.Distance(Iter2.Value) +
										        Iter2.Value.Distance(Iter1.Value) +
										        Iter1.Value.Distance(Iter3.Value);

			        if (dCurrentPerimPart > dNewPerimPart)
			        {
				        C2DPoint pRem = Iter2.Value;
                        this.Remove(Iter2);
                        this.AddBefore(Iter1, pRem);
				  //      m_Points->erase(Iter2);
				   //     m_Points->insert( Iter1, pRem );
				        bRepeat = true;
			        }


			        Iter = Iter.Next;
			        nIndex++;
		        }	

	        }
        }

        /// <summary>
	    /// Brute force optimisation
        /// </summary>
        public void SimpleReorder()
        {
	        // CHECK FOR LESS THAN 2 ITEMS.
	        if (Count < 2)
		        return;

            LinkedListNode<C2DPoint> IterRemove = First;
            IterRemove = IterRemove.Next;
            LinkedListNode<C2DPoint> IterRemoveLimit = Last;

	   //     std::list<C2DPoint*>::iterator IterRemove = m_Points->begin(); // The item considered for removal.
	   //     IterRemove++; // We don't want to remove the first point
	    //    std::list<C2DPoint*>::iterator IterRemoveLimit = m_Points->end(); // The last item considered for removal.
	    //    IterRemoveLimit--; // We don't want to remove the end. (This IS the end but we won't go that far)
        		

	        while (IterRemove != IterRemoveLimit)
	        {
                LinkedListNode<C2DPoint> IterInsert = First;
                IterInsert = IterInsert.Next;
            //    LinkedListNode<C2DPoint> IterInsertLimit = First;

		        // Cycle through the rest to find a new place.
		  //      std::list<C2DPoint*>::iterator IterInsert = m_Points->begin(); // The item considered for a new place.
		  //      IterInsert++; // We don't want to insert at the start.
		  //      std::list<C2DPoint*>::iterator IterInsertLimit = m_Points->end(); // The limit for a new place.

		        while (IterInsert != null)
		        {
			        if (IterInsert == IterRemove)
			        {
				        IterInsert  = IterInsert.Next;
				        continue; // No point removing it and putting back in the same place.
			        }

                    LinkedListNode<C2DPoint> IterRemoveBefore = IterRemove;
                    IterRemoveBefore = IterRemoveBefore.Previous;
                    LinkedListNode<C2DPoint> IterRemoveAfter = IterRemove;
                    IterRemoveAfter = IterRemoveAfter.Next;
                    LinkedListNode<C2DPoint> IterInsertBefore = IterInsert;
                    IterInsertBefore = IterInsertBefore.Previous;


			//        std::list<C2DPoint*>::iterator IterRemoveBefore = IterRemove;
			  //      IterRemoveBefore--; // This is the point before the removal
			   //     std::list<C2DPoint*>::iterator IterRemoveAfter = IterRemove;
			   //     IterRemoveAfter++; // This is the point after the removal.
			  //      std::list<C2DPoint*>::iterator IterInsertBefore = IterInsert;
			//        IterInsertBefore -- ;// This is the point before the potential insertion point.

                    double dCurrentPerimPart = IterRemoveBefore.Value.Distance(IterRemove.Value) +
                                               IterRemove.Value.Distance(IterRemoveAfter.Value) +
                                               IterInsertBefore.Value.Distance(IterInsert.Value);
                    double dNewPerimPart = IterRemoveBefore.Value.Distance(IterRemoveAfter.Value) +
                                            IterInsertBefore.Value.Distance(IterRemove.Value) +
                                            IterRemove.Value.Distance(IterInsert.Value);
			        if (dNewPerimPart < dCurrentPerimPart)
			        {
                        C2DPoint ptRemove = IterRemove.Value;
                        this.Remove(IterRemove);
                        this.AddBefore(IterInsert, ptRemove);

				  //      m_Points->erase(IterRemove);
				  //      m_Points->insert( IterInsert, ptRemove );
				        break;
			        }
                    IterInsert = IterInsert.Next;

		        }

                IterRemove = IterRemove.Next;
	        }


        }

        /// <summary>
	    /// Optimises the position of the points
        /// </summary>
        public void Optimize()
        {
            if (Count < 4)
                return;

            // Take out the start.
            C2DPoint pStart = First.Value;
            this.RemoveFirst();
          //  m_Points->pop_front();

            // Take out the end.
            C2DPoint pEnd = Last.Value;
            this.RemoveLast();
        //    m_Points->pop_back();

            // Take all the rest out.
            C2DPointSet Points = new C2DPointSet();
            this.ExtractPoints(Points);

            // Put the ends back in.
            this.AddFirst(pStart);
            this.AddLast(pEnd);
        //    m_Points->push_front(pStart);
        //    m_Points->push_back(pEnd);

            // Sort the rest by approx distance from the line in reverse order so we can get them off the end.
            Points.SortByDistance(pStart.GetMidPoint(pEnd));
            Points.Reverse();

            // Add them all in the most sensible place (not gauranteed).
            while (Points.Count > 0)
            {
                this.InsertOptimally(Points[Points.Count - 1]);
                Points.RemoveAt(Points.Count - 1);
            }

        }

        private const int conRefineProximity = 10;
        private const int conMaxIterations = 5;
    }
}
