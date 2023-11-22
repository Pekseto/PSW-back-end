﻿using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Enums;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Converters;
using Explorer.BuildingBlocks.Core.UseCases;
using FluentResults;
using static Explorer.Blog.API.Enums.BlogEnums;

namespace Explorer.Blog.Core.UseCases
{
    public class BlogService : CrudService<BlogDto, Domain.Blog> ,IBlogService
    {
        public BlogService(ICrudRepository<Domain.Blog> repository, IMapper mapper): base(repository, mapper) { }

        public Result<List<BlogDto>> GetFiltered(BlogStatus filter)
        {
            var filtered = new List<Domain.Blog>();
            switch (filter)
            {
                case BlogStatus.Active: filtered = CrudRepository.GetFiltered(isActive);
                    break;
                case BlogStatus.Famous: filtered = CrudRepository.GetFiltered(isFamous);
                    break;
                default: filtered = CrudRepository.GetFiltered(isPublished);
                    break;
            }
            return MapToDto(filtered);
        }

        public Result<BlogDto> RateBlog(int blogId, BlogRatingDto rating)
        {
            var ratingDomain = BlogRatingConverter.ToDomain(rating);
            var oldBlog = CrudRepository.Get(blogId);
            oldBlog.Rate(ratingDomain);
            CrudRepository.Update(oldBlog);
            return MapToDto(oldBlog);
        }
        public Result<BlogDto> PublishBlog(int blogId)
        {
            var toPublish = CrudRepository.Get(blogId);
            toPublish.PublishBlog();
            CrudRepository.Update(toPublish);
            return MapToDto(toPublish);
        }
        public Result<BlogDto> CommentBlog(int blogId, BlogCommentDto comment)
        {
            var blogComment = BlogCommentConverter.ToDomain(comment);
            Domain.Blog blog = CrudRepository.Get(blogId);
            blog.BlogComments.Add(blogComment);
            CrudRepository.Update(blog);
            return MapToDto(blog);
        }
        public Result<BlogDto> UpdateComment(int blogId, BlogCommentDto comment)
        {
            var blogComment = BlogCommentConverter.ToDomain(comment);
            var blog = CrudRepository.Get(blogId);
            blog.UpdateComments(blogComment);
            CrudRepository.Update(blog);
            return MapToDto(blog);
        }
        public Result<BlogDto> DeleteComment(int blogId, BlogCommentDto comment)
        {
            var blogComment = BlogCommentConverter.ToDomain(comment);
            var blog = CrudRepository.Get(blogId);
            blog.BlogComments.Remove(blogComment);
            CrudRepository.Update(blog);
            return MapToDto(blog);
        }

        private Predicate<Domain.Blog> isFamous = blog => blog.Status == BlogEnums.BlogStatus.Famous;
        private Predicate<Domain.Blog> isActive = blog => blog.Status == BlogEnums.BlogStatus.Active;
        private Predicate<Domain.Blog> isEligible = blog => blog.Status != BlogStatus.Draft;

    }
}
