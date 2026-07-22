/*
Copyright 2026 Diogo Esteves, Guilherme Mattos

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System.Collections.Generic;

namespace SpookyTattoos.Application.DTOs.Posts;

public class CreatePostImageDto
{   
    public  int PostId {get; set;}
    public required string ImageUrl { get; set; }
    public int DisplayOrder { get; set; } = 1;
}

public class CreatePostDto
{
    public required int JobId { get; set; }
    public string? Description { get; set; }
    public string? PostText { get; set; }
    
    public IEnumerable<CreatePostImageDto> Images { get; set; } = new List<CreatePostImageDto>();
}