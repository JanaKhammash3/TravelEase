﻿namespace TravelEase.TravelEase.Application.DTOs
{
    public class ReviewResponseDto
    {
        public string UserName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}