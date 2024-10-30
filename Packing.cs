using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RectiPack {

    public static class Packing {

        /// <summary>
        /// Used to select one or both of the dimensions for a rectangle
        /// </summary>
        public enum Dimensions {
            Both,
            Width,
            Height
        }

        internal static Tuple<Rect?, long> FindBestPacking(Rect[] rects, PackingGrid root, Rect startingBin, int discardStep, Dimensions triedDimensions) {
            Dictionary<Rect, Tuple<bool, long>> cache = new Dictionary<Rect, Tuple<bool, long>>();
            

            Rect candidateBin = startingBin;
            int triesBeforeDiscarding = 0;

            // Negative discard steps result in multiple tries with a discardStep value of 1
            if (discardStep <= 0) {
                triesBeforeDiscarding = -discardStep;
                discardStep = 1;
            }



            uint startingStep = 0;

            // Figure out along which axis the rect can shrink
            switch (triedDimensions) {
                
                case Dimensions.Both:
                    candidateBin.width /= 2;
                    candidateBin.height /= 2;

                    startingStep = candidateBin.width / 2;
                    break;

                case Dimensions.Width:
                    candidateBin.width /= 2;

                    startingStep = candidateBin.width / 2;
                    break;

                case Dimensions.Height:
                    candidateBin.height /= 2;

                    startingStep = candidateBin.height / 2;
                    break;
            }



            for (uint step = startingStep; true; step = Math.Max(1, step / 2)) {
                #if ENABLE_RECTIPACK_LOGGING
                    Console.Write("Step: " + step + " \t" + candidateBin + "\t");
                #endif

                // Reset the root area
                root.Reset(candidateBin);

                long totalInsertedArea = 0;
                bool allRectsInserted = true;

                // Check if the current candidate is in the cache
                if (cache.ContainsKey(candidateBin)) {
                    Tuple<bool, long> data = cache[candidateBin];
                    
                    allRectsInserted = data.Item1;
                    totalInsertedArea = data.Item2;

                } else {

                    // Calculate current candidate and add them to the cache
                    foreach (Rect rect in rects) {
                        if (root.Insert(rect) == null) { allRectsInserted = false; break; }
                        totalInsertedArea += rect.Area();
                    }

                    // Add the current result to the cache
                    cache.Add(candidateBin, new Tuple<bool, long>(allRectsInserted, totalInsertedArea));
                }

                #if ENABLE_RECTIPACK_LOGGING    
                    Console.Write("Fits: " + allRectsInserted + "\t");
                    //root.PrintFreeSpaces();       // Tested this, and the number of empty spaces seems reasonable in most cases, so no need to clutter the console even more
                    Console.WriteLine();
                #endif


                if (allRectsInserted) {
                    // Attempt was successful, now shrink the bin and try again

                    // Check if the limit of attempts is reached
                    if (step <= discardStep) {
                        if (triesBeforeDiscarding > 0) { triesBeforeDiscarding--; }
                        else { return new Tuple<Rect?, long>(candidateBin, totalInsertedArea); }
                    }

                    // Otherwise shrink the bin
                    switch (triedDimensions) {
                        
                        case Dimensions.Both:
                            candidateBin.width -= step;
                            candidateBin.height -= step;
                            break;

                        case Dimensions.Width:
                            candidateBin.width -= step;
                            break;

                        case Dimensions.Height:
                            candidateBin.height -= step;
                            break;
                    }

                    root.Reset(candidateBin);
                
                } else {

                    // Attempt failed, increase the size of the bin

                    switch (triedDimensions) {

                        case Dimensions.Both:
                            candidateBin.width += step;
                            candidateBin.height += step;

                            if (candidateBin.Area() > startingBin.Area()) { return new Tuple<Rect?, long>(null, totalInsertedArea); }    // Check if the size of the starting bin has been exceeded
                            break;

                        case Dimensions.Width:
                            candidateBin.width += step;

                            if (candidateBin.width > startingBin.width) { return new Tuple<Rect?, long>(null, totalInsertedArea); }    // Check if the size of the starting bin has been exceeded
                            break;

                        case Dimensions.Height:
                            candidateBin.height += step;

                            if (candidateBin.height > startingBin.height) { return new Tuple<Rect?, long>(null, totalInsertedArea); }    // Check if the size of the starting bin has been exceeded
                            break;
                    }
                }

            }


        }



    }


}
