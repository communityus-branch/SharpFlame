

using System.Diagnostics;


namespace SharpFlame.Pathfinding
{
    public class PathfinderConnection
    {
        public int CalcValueNum = -1;
        private PathfinderConnection DependantConnection; //the one above this that partially relies on this to exist
        public bool Destroyed;

        public int Layer_ConnectionNum = -1;
        private int LinkCount;

        public PathfinderNode NodeA;

        public int NodeA_ConnectionNum = -1;

        public PathfinderNode NodeB;

        public int NodeB_ConnectionNum = -1;

        public float Value = 1.0F;

        public PathfinderConnection(PathfinderNode NewNodeA, PathfinderNode NewNodeB, float NewValue)
        {
            if ( NewNodeA.Layer.Network_LayerNum > 0 | NewNodeB.Layer.Network_LayerNum > 0 | NewValue <= 0.0F )
            {
                Debugger.Break();
                return;
            }

            Value = NewValue;

            LinkCount = 1;

            NodeA = NewNodeA;
            NodeB = NewNodeB;
            NodeA.Connection_Add(this, ref NodeA_ConnectionNum);
            NodeB.Connection_Add(this, ref NodeB_ConnectionNum);

            NodeA.Layer.Connection_Add(this);

            RaiseDependant();
        }

        public PathfinderConnection(PathfinderConnection SourceConnection)
        {
            NodeA = SourceConnection.NodeA.ParentNode;
            NodeB = SourceConnection.NodeB.ParentNode;
            NodeA.Connection_Add(this, ref NodeA_ConnectionNum);
            NodeB.Connection_Add(this, ref NodeB_ConnectionNum);

            NodeA.Layer.Connection_Add(this);
            ValueCalc();
        }

        public PathfinderNode GetNodeA
        {
            get { return NodeA; }
        }

        public int GetNodeA_ConnectionNum
        {
            get { return NodeA_ConnectionNum; }
        }

        public PathfinderNode GetNodeB
        {
            get { return NodeB; }
        }

        public int GetNodeB_ConnectionNum
        {
            get { return NodeB_ConnectionNum; }
        }

        public float GetValue
        {
            get { return Value; }
        }

        private void RemoveFromNodes()
        {
            NodeA.Connection_Remove(NodeA_ConnectionNum);
            NodeA = null;
            NodeA_ConnectionNum = -1;

            NodeB.Connection_Remove(NodeB_ConnectionNum);
            NodeB = null;
            NodeB_ConnectionNum = -1;
        }

        public PathfinderNode GetOtherNode(PathfinderNode Self)
        {
            if ( NodeA == Self )
            {
                return NodeB;
            }
            return NodeA;
        }

        private void LinkIncrease()
        {
            LinkCount++;
        }

        private void LinkDecrease()
        {
            LinkCount--;
            if ( LinkCount == 0 )
            {
                Destroy();
            }
            else if ( LinkCount < 0 )
            {
                Debugger.Break();
            }
        }

        public void RaiseDependant()
        {
            var tmpConnectionA = default(PathfinderConnection);

            if ( DependantConnection != null )
            {
                return;
            }

            if ( NodeA.ParentNode != NodeB.ParentNode )
            {
                if ( NodeA.ParentNode != null && NodeB.ParentNode != null )
                {
                    tmpConnectionA = NodeA.ParentNode.FindConnection(NodeB.ParentNode);
                    if ( tmpConnectionA == null )
                    {
                        DependantConnection = new PathfinderConnection(this);
                        DependantConnection.LinkIncrease();
                        DependantConnection.RaiseDependant();
                    }
                    else
                    {
                        DependantConnection = tmpConnectionA;
                        DependantConnection.LinkIncrease();
                    }
                }
            }
        }

        public void Destroy()
        {
            if ( Destroyed )
            {
                return;
            }
            Destroyed = true;

            var Layer = NodeA.Layer;

            var tmpA = NodeA.ParentNode;
            var tmpB = NodeB.ParentNode;
            RemoveFromNodes();
            if ( tmpA != null )
            {
                tmpA.CheckIntegrity();
            }
            if ( tmpB != null && tmpB != tmpA )
            {
                tmpB.CheckIntegrity();
            }
            UnlinkParentDependants();
            Layer.Connection_Remove(Layer_ConnectionNum);
        }

        public void ForceDeallocate()
        {
            DependantConnection = null;
            NodeA = null;
            NodeB = null;
        }

        public void UnlinkParentDependants()
        {
            if ( DependantConnection != null )
            {
                var tmpConnection = DependantConnection;
                DependantConnection = null;
                tmpConnection.LinkDecrease();
            }
        }

        public void ValueCalc()
        {
            if ( NodeA.Layer.Network_LayerNum == 0 )
            {
                Debugger.Break();
            }

            var Args = new PathfinderNetwork.sFloodForValuesArgs();
            var A = 0;
            var NumA = 0;
            float TotalValue = 0;
            Args.NodeValues = NodeA.Layer.Network.NetworkLargeArrays.Nodes_ValuesA;
            Args.FinishIsParent = false;
            Args.SourceNodes = NodeA.Layer.Network.NetworkLargeArrays.Nodes_Nodes;
            Args.SourceParentNodeA = NodeA;
            Args.SourceParentNodeB = NodeB;
            Args.CurrentPath = NodeA.Layer.Network.NetworkLargeArrays.Nodes_Path;
            Args.FinishNodeCount = NodeB.NodeCount;
            Args.FinishNodes = new PathfinderNode[NodeB.NodeCount];
            for ( A = 0; A <= NodeB.NodeCount - 1; A++ )
            {
                Args.FinishNodes[A] = NodeB.Nodes[A];
            }
            for ( NumA = 0; NumA <= NodeA.NodeCount - 1; NumA++ )
            {
                Args.CurrentPath.Nodes[0] = NodeA.Nodes[NumA];
                Args.CurrentPath.NodeCount = 1;
                for ( A = 0; A <= NodeA.NodeCount - 1; A++ )
                {
                    Args.NodeValues[NodeA.Nodes[A].Layer_NodeNum] = float.MaxValue;
                }
                for ( A = 0; A <= NodeB.NodeCount - 1; A++ )
                {
                    Args.NodeValues[NodeB.Nodes[A].Layer_NodeNum] = float.MaxValue;
                }
                Args.BestPaths = new Path[Args.FinishNodeCount];
                NodeA.Layer.Network.FloodForValues(ref Args);
                for ( A = 0; A <= NodeB.NodeCount - 1; A++ )
                {
                    if ( Args.BestPaths[A] == null )
                    {
                        Debugger.Break();
                        return;
                    }
                    TotalValue += Args.BestPaths[A].Value;
                }
            }
            Value = TotalValue / (NodeA.NodeCount * NodeB.NodeCount);
            if ( Value == 0.0F )
            {
                Debugger.Break();
            }

            CalcValueNum = -1;
        }
    }
}