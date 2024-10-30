/*
 *      This implementation is heavily based on:
 *      https://github.com/TeamHypersomnia/rectpack2D/tree/master
 * 
 *      
 *      The goal is to fit a given number of rectangles into the smallest possible rectangular area without rotation/flipping sides.
 *      Logging to console can be enabled by adding "ENABLE_RECTIPACK_LOGGING" to the conditional compilation symbols of this project.
 * 
 */

namespace RectiPack {

    /// <summary>
    /// Represents a rectangle defined by its width and height
    /// </summary>
    public struct Rect {

        public uint width, height;


        public long Area() { checked { return (long)width * (long)height; } }
        
        // Has to create a new rect since "Structs in C# are immutable"...
        internal Rect ExpandWith(AreaRect rect) {
            return new Rect() {
                width = Math.Max(width, rect.x + rect.width),
                height = Math.Max(height, rect.y + rect.height)
            };
        }


        #region Operator overloads
        public static bool operator ==(Rect a, Rect b) { return a.width == b.width && a.height == b.height; }
        public static bool operator !=(Rect a, Rect b) { return !(a == b); }

        public override bool Equals(object? obj) {
            var item = obj as Rect?;

            if (item == null) { return false; }

            return this == item;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }


        public override string ToString() { return $"{width}, {height}"; }
        #endregion
    }

    /// <summary>
    /// Represents a rectangle in a coordinate system defined by its x and y position and its width and height
    /// </summary>
    public struct AreaRect {
        public uint x, y, width, height;

        public uint Area() { checked { return width * height; } }

        #region Operator Overloads
        public static bool operator ==(AreaRect a, AreaRect b) { return a.x == b.x && a.y == b.y && a.width == b.width && a.height == b.height; }
        public static bool operator !=(AreaRect a, AreaRect b) { return !(a == b); }

        public override bool Equals(object? obj) {
            var item = obj as AreaRect?;

            if (item == null) { return false; }

            return this == item;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public override string ToString() { return $"{x}, {y}, {width}, {height}"; }
        #endregion
    }


    /// <summary>
    /// Contains the result of the packing function
    /// </summary>
    public struct Package {
        
        /// <summary>
        /// The area everything got packed into
        /// </summary>
        public Rect boundingRect;

        /// <summary>
        /// The placement for every rect from the input in the exact same order as the input rects
        /// (i.e. the rect at index 3 in the input corresponds to the rect at index 3 in placements)
        /// </summary>
        public AreaRect[] placements;

        /// <summary>
        /// The comparer used to create this outcome (Mostly used for debugging)
        /// </summary>
        public Comparer<Rect> usedOrder;
    }

    
    /// <summary>
    /// Contains all functionality related to packing rects
    /// </summary>
    public static class RectiPack {

        /// <summary>
        /// All built-in rectangle sorting functions
        /// </summary>
        public static readonly Comparer<Rect>[] DEFAULT_SORTING_ORDER = [Comparators.OrderByArea, Comparators.OrderByPerimeter, Comparators.OrderByBiggerSide, Comparators.OrderByWidth, Comparators.OrderByHeight];

        #region Overloads
        /// <summary>
        /// Calculates the smallest rectangular area all given rectangles can fit in without rotating/flipping them
        /// </summary>
        /// <param name="rects">The list of rectangles</param>
        /// <returns>Null on failure, otherwise a Package struct containing the minimum area and the placement of each rectangle in the same order as in the input</returns>
        public static Package? Pack(Rect[] rects) {
            return Pack(rects, -1);
        }

        /// <summary>
        /// Calculates the smallest rectangular area all given rectangles can fit in without rotating/flipping them
        /// </summary>
        /// <param name="rects">The list of rectangles</param>
        /// <param name="discardStep">The minimum step size for reducing the enclosing area before aborting. Smaller Values result in better packing. Negative values use a stepsize of 1 and allow more retries at the same size. Most of the times -1 is enough for the best packing</param>
        /// <returns>Null on failure, otherwise a Package struct containing the minimum area and the placement of each rectangle in the same order as in the input</returns>
        public static Package? Pack(Rect[] rects, int discardStep) {
            return Pack(rects, int.MaxValue, discardStep);
        }

        /// <summary>
        /// Calculates the smallest rectangular area all given rectangles can fit in without rotating/flipping them
        /// </summary>
        /// <param name="rects">The list of rectangles</param>
        /// <param name="maxBinSize">The maximum size of the square packing area. If this value is exceeded, the packing fails.</param>
        /// <param name="discardStep">The minimum step size for reducing the enclosing area before aborting. Smaller Values result in better packing. Negative values use a stepsize of 1 and allow more retries at the same size. Most of the times -1 is enough for the best packing</param>
        /// <returns>Null on failure, otherwise a Package struct containing the minimum area and the placement of each rectangle in the same order as in the input</returns>
        public static Package? Pack(Rect[] rects, int maxBinSize, int discardStep) {
            return Pack(rects, maxBinSize, discardStep, DEFAULT_SORTING_ORDER, Packing.Dimensions.Both);
        }

        #endregion


        /// <summary>
        /// Calculates the smallest rectangular area all given rectangles can fit in without rotating/flipping them
        /// </summary>
        /// <param name="inputRects">The list of rectangles</param>
        /// <param name="maxBinSize">The maximum size of the square packing area. If this value is exceeded, the packing fails.</param>
        /// <param name="discardStep">The minimum step size for reducing the enclosing area before aborting. Smaller Values result in better packing. Negative values use a stepsize of 1 and allow more retries at the same size. Most of the times -1 is enough for the best packing</param>
        /// <param name="orderBy">The order in which different sorting mechanisms for the rects are tried.</param>
        /// <param name="adjustableDims">The dimensions along which the packing area's size can be changed.</param>
        /// <returns>Null on failure, otherwise a Package struct containing the minimum area and the placement of each rectangle in the same order as in the input</returns>
        public static Package? Pack(Rect[] inputRects, int maxBinSize, int discardStep, Comparer<Rect>[] orderBy, Packing.Dimensions adjustableDims) {
            
            // The maximum size everything should fit into
            Rect maxBin = new Rect() { width = (uint)maxBinSize, height = (uint)maxBinSize };

            // Create the grid on which each packing attempt will be performed
            PackingGrid root = new PackingGrid(new Rect());

            // Placeholder for a copy of all rects - All sorting operations are performed on a copy of the input array so that the original won't not altered
            Rect[] rects;

            long bestTotalInserted = -1;        // The largest area that has been filled in after a failed attempt - currently only used for additional logging
            Rect bestBin = maxBin;
            Comparer<Rect> bestOrder = null;

            // Try each sorting approach and keep record of the best performing one.
            foreach (Comparer<Rect> order in orderBy) {
                #if ENABLE_RECTIPACK_LOGGING
                    Console.WriteLine("\nAttempting " + order);
                #endif

                // Copy & sort the array
                // I did some testing and found out that copying the array everytime (which results in some memory overhead) is ~2x faster than using a deterministic sorting function
                // that always produces the same results, while also achieving (basically) deterministic results.
                rects = new Rect[inputRects.Length];
                Array.Copy(inputRects, rects, rects.Length);
                
                Array.Sort(rects, order);

                // Try to pack everything
                Tuple<Rect?, long> packing = Packing.FindBestPacking(rects, root, maxBin, discardStep, adjustableDims);

                if (packing.Item1 == null) {
                    // The function failed - Just keep record of the filled area
                    if (packing.Item2 > bestTotalInserted) { bestTotalInserted = packing.Item2; }

                } else {
                    // The function succeeded
                    Rect resultingRect = (packing.Item1 ?? maxBin);

                    // Check if the current attempts performed the best so far
                    if (resultingRect.Area() < bestBin.Area()) {
                        bestBin = resultingRect;
                        bestOrder = order;
                    }
                }
            }

            // Could not find a rectangle that satisfies all conditions
            if (bestOrder == null) {
                #if ENABLE_RECTIPACK_LOGGING
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Failed to pack rectangles. Best attempt managed to fill in {bestTotalInserted} square units");
                    Console.ForegroundColor = ConsoleColor.White;
                #endif

                return null; 
            }

            // If the function succeeded:
            // Package everything into the best possible bin
            root.Reset(bestBin);

            // Create a new copy and a second array to keep track of the index for each rectangle after sorting
            rects = new Rect[inputRects.Length];
            Array.Copy(inputRects, rects, rects.Length);

            int[] indices = new int[inputRects.Length];
            for (int i = 0; i < indices.Length; i++) { indices[i] = i; }

            // Sort both array together
            Array.Sort(rects, indices, bestOrder);


            AreaRect[] placements = new AreaRect[rects.Length];

            // Reconstruct the placement for each rectangle that resulted in bestBin (and translate all indices so that the resulting placement array and the input array line up again)
            for (int i = 0; i < rects.Length; i++) {
                placements[indices[i]] = root.Insert(rects[i]) ?? throw new Exception("Failed to reassemble rects");
            }

            // And finally return the found solution
            return new Package() { boundingRect = root.currentSpanningRect, placements = placements, usedOrder = bestOrder };
        }

    }

}
