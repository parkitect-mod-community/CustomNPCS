using System;
using BehaviourTree;
using System.Collections.Generic;

namespace HelloMod
{
    public class RandomPath : Node
    {
        private Person _person;

        private DepthFirstSearch _search;
        private string _path;

        public RandomPath (string outPath)
        {
            _path = outPath;
        }

        public override void initialize (DataContext dataContext)
        {
            base.initialize (dataContext);

            _person = dataContext.person;
            _search = new DepthFirstSearch (5, _person.transform.position, _person);
        }

        protected override Result run (DataContext dataContext)
        {
            _search.run ();
            List<Pathfinding.PathNode> path = this._search.path;
            dataContext.set(_path, path);
            if (path != null)
            {
                if (path.Count > 0)
                {
                    path.RemoveAt(path.Count - 1);
                }
                return Node.Result.SUCCESS;
            }
            return Node.Result.FAILED;
        }
    }
}

