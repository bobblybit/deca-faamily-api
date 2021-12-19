
using System.Collections.Generic;

namespace AccountManagement.Data.Dtos
{
    public class StacksAndSquadsToReturnDto
    {
        public IEnumerable<StackToReturnDto> Stacks { get; set; } = new HashSet<StackToReturnDto>();
        public IEnumerable<SquadToReturnDto> Squads { get; set; } = new HashSet<SquadToReturnDto>();
    }
}
