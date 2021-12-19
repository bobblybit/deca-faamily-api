using System.Collections.Generic;

namespace MqDtos
{
    public class DbSeedMqDto
    {
        public List<SeededUserMqDto> Users { get; set; } = new List<SeededUserMqDto>();
        public List<SeededStackMqDto> Stacks { get; set; } = new List<SeededStackMqDto>();
    }
}
