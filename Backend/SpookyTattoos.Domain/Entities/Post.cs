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

namespace SpookyTattoos.Domain.Entities;

public class PostImage
{
    public int Id { get; set; }
    
    public required int PostId { get; set; }
    public Post? Post { get; set; }

    public required string ImageUrl { get; set; }
    
    private int _displayOrder = 1;
    public int DisplayOrder 
    {
        get => _displayOrder;
        set
        {
            if (value < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(DisplayOrder), "A ordem de exibição tem de ser maior que 0.");
            }
            _displayOrder = value;
        }
    }
}

public class Post
{
    public int Id { get; set; }
    
    public required int JobId { get; set; }
    public Job? Job { get; set; }

    public string? Description { get; set; }
    public string? PostText { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public bool IsPublished { get; set; } = false;

    public ICollection<PostImage> Images { get; set; } = new List<PostImage>();


    public void ChangeState()
    {
        if(IsPublished) IsPublished = false;
        IsPublished = true;
    }
}